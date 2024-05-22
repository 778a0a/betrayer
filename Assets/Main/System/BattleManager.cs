using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class BattleManager
{
    /// <summary>
    /// 戦闘を行います。
    /// </summary>
    public static async ValueTask<BattleResult> Battle(
        MapGrid map,
        Area sourceArea,
        Area targetArea,
        Character attacker,
        Character defender)
    {
        var dir = sourceArea.GetDirectionTo(targetArea);
        var attackerTerrain = map.Helper.GetAttackerTerrain(sourceArea.Position, dir);
        var defenderTerrain = targetArea.Terrain;
        Debug.Log($"[戦闘処理] {attacker?.Name}({attacker?.Power}) -> {defender?.Name}({defender?.Power}) at {sourceArea?.Position} -> {targetArea?.Position}");
        Debug.Log($"[戦闘処理] 攻撃側地形: {attackerTerrain} 防御側地形: {defenderTerrain}");
        if (defender == null)
        {
            Debug.Log($"[戦闘処理] 防御側がいないので侵攻側の勝利です。");
            return BattleResult.AttackerWin;
        }

        var world = GameCore.Instance.World;
        var atk = new CharacterInBattle(attacker, attackerTerrain, world.CountryOf(attacker), true);
        var def = new CharacterInBattle(defender, defenderTerrain, world.CountryOf(defender), false);
        atk.Opponent = def;
        def.Opponent = atk;

        var battle = new Battle(atk, def, world);
        var result = await battle.Do();
        return result;
    }

    /// <summary>
    /// 戦闘後の回復処理
    /// </summary>
    public static void Recover(Character c, bool win)
    {
        foreach (var s in c.Force.Soldiers)
        {
            if (!s.IsAlive) continue;

            var baseAmount = s.MaxHp * (win ? 0.1f : 0.05f);
            var adj = Mathf.Max(0, (c.Intelligence - 80) / 100f / 2);
            var amount = (int)(baseAmount * (1 + adj));
            s.Hp = Mathf.Min(s.MaxHp, s.Hp + amount);
        }
    }
}


public record CharacterInBattle(
    Character Character,
    Terrain Terrain,
    Country Country,
    bool IsAttacker)
{
    public CharacterInBattle Opponent { get; set; }
    public bool IsDefender => !IsAttacker;

    /// <summary>
    /// 戦闘の強さ
    /// 攻撃側ならAttack、防御側ならDefense
    /// </summary>
    public int Strength => IsAttacker ? Character.Attack : Character.Defense;

    public Force Force => Character.Force;
    public bool IsPlayer => Character.IsPlayer;
    public bool AllSoldiersDead => Character.Force.Soldiers.All(s => !s.IsAlive);

    public static implicit operator Character(CharacterInBattle c) => c.Character;

    public bool ShouldRetreat()
    {
        // プレーヤーの場合はUIで判断しているので処理不要。
        if (IsPlayer) return false;

        // まだ損耗が多くないなら撤退しない。
        var manyLoss = Character.Force.Soldiers.Count(s => s.Hp < 10) >= 3;
        if (!manyLoss) return false;

        // 敵よりも兵力が多いなら撤退しない。
        var myPower = Force.Power;
        var opPower = Opponent.Force.Power;
        if (myPower > opPower) return false;

        // 敵に残り数の少ない兵士がいるなら撤退しない。
        var opAboutToDie = Opponent.Force.Soldiers.Any(s => s.Hp <= 3);
        if (opAboutToDie) return false;

        // 自国の最後の領土なら撤退しない。
        var lastArea = Country.Areas.Count == 1;
        if (lastArea) return false;

        // 撤退する。
        return true;
    }
}


public class Battle
{
    public CharacterInBattle Attacker { get; set; }
    public CharacterInBattle Defender { get; set; }
    private int TickCount { get; set; }
    
    private WorldData World { get; set; }
    private BattleDialog UI => GameCore.Instance.MainUI.BattleDialog;
    private bool NeedUI => Attacker.IsPlayer || Defender.IsPlayer;

    public Battle(CharacterInBattle atk, CharacterInBattle def, WorldData world)
    {
        this.Attacker = atk;
        this.Defender = def;
        World = world;
    }

    public async ValueTask<BattleResult> Do()
    {
        if (NeedUI)
        {
            UI.Root.style.display = DisplayStyle.Flex;
            UI.SetData(this);
        }

        var result = default(BattleResult);
        while (!Attacker.AllSoldiersDead && !Defender.AllSoldiersDead)
        {
            // 撤退判断を行う。
            if (NeedUI)
            {
                UI.SetData(this);
                var shouldContinue = await UI.WaitPlayerClick();
                if (!shouldContinue)
                {
                    result = Attacker.IsPlayer ?
                        BattleResult.DefenderWin :
                        BattleResult.AttackerWin;
                    break;
                }
            }
            if (Attacker.ShouldRetreat())
            {
                result = BattleResult.DefenderWin;
                break;
            }
            if (Defender.ShouldRetreat())
            {
                result = BattleResult.AttackerWin;
                break;
            }

            Tick();
        }

        if (result == BattleResult.None)
        {
            result = Attacker.AllSoldiersDead ?
                BattleResult.DefenderWin :
                BattleResult.AttackerWin;
        }
        Debug.Log($"[戦闘処理] 結果: {result}");
        
        // 画面を更新する。
        if (NeedUI)
        {
            UI.SetData(this);
            await UI.WaitPlayerClick();
            UI.Root.style.display = DisplayStyle.None;
        }

        // 死んだ兵士のスロットを空にする。
        foreach (var sol in Attacker.Force.Soldiers.Concat(Defender.Force.Soldiers))
        {
            if (sol.Hp == 0)
            {
                sol.IsEmptySlot = true;
            }
        }

        return result;
    }

    public static float TerrainDamageAdjustment(Terrain t) => t switch
    {
        Terrain.LargeRiver => 0.40f,
        Terrain.River => 0.25f,
        Terrain.Plain => 0.0f,
        Terrain.Hill => -0.1f,
        Terrain.Forest => -0.25f,
        Terrain.Mountain => -0.40f,
        Terrain.Fort => -0.50f,
        _ => 0f,
    };

    private void Tick()
    {
        // 両方の兵士をランダムな順番の配列にいれる。
        var all = Attacker.Force.Soldiers.Select(s => (soldier: s, owner: Attacker))
            .Concat(Defender.Force.Soldiers.Select(s => (soldier: s, owner: Defender)))
            .Where(x => x.soldier.IsAlive)
            .ToArray()
            .ShuffleInPlace();

        var baseAdjustment = new Dictionary<Character, float>
        {
            {Attacker, BaseAdjustment(Attacker, TickCount)},
            {Defender , BaseAdjustment(Defender, TickCount)},
        };
        static float BaseAdjustment(CharacterInBattle chara, int tickCount)
        {
            var op = chara.Opponent;
            var adj = 1f;
            adj += (chara.Strength - 50) / 100f;
            adj -= (op.Strength - 50) / 100f;
            adj += (chara.Character.Intelligence - 50) / 100f * Mathf.Min(1, tickCount / 10f);
            adj -= (op.Character.Intelligence - 50) / 100f * Mathf.Min(1, tickCount / 10f);
            adj += TerrainDamageAdjustment(op.Terrain);
            return adj;
        }
        Debug.Log($"[戦闘処理] 基本調整値: atk:{baseAdjustment[Attacker]:0.00} def:{baseAdjustment[Defender]:0.00}");

        var attackerTotalDamage = 0f;
        var defenderTotalDamage = 0f;
        foreach (var (soldier, owner) in all)
        {
            var opponent = owner.Opponent;
            if (!soldier.IsAlive) continue;
            var target = opponent.Force.Soldiers.Where(s => s.IsAlive).RandomPickDefault();
            if (target == null) continue;

            var adj = baseAdjustment[owner];
            adj += Random.Range(-0.2f, 0.2f);
            adj += soldier.Level / 10f;

            var damage = Math.Max(0, adj);
            target.HpFloat = (int)Math.Max(0, target.HpFloat - damage);

            soldier.Experience += 1 + Random.Range(0, 0.3f);
            // 十分経験値が貯まればレベルアップする。
            if (soldier.Experience >= soldier.Level * 10 && soldier.Level < 13)
            {
                soldier.Level += 1;
                soldier.Experience = 0;
            }

            if (owner.IsAttacker)
            {
                attackerTotalDamage += damage;
            }
            else
            {
                defenderTotalDamage += damage;
            }
        }

        if (Attacker.IsPlayer || Defender.IsPlayer)
        {
            Debug.Log($"[戦闘処理] " +
                $"{Attacker.Character.Name}の総ダメージ: {attackerTotalDamage} " +
                $"{Defender.Character.Name}の総ダメージ: {defenderTotalDamage}");
        }
    }
}
