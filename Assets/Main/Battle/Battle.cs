using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine;
using Random = UnityEngine.Random;

public class Battle
{
    public CharacterInBattle Attacker { get; set; }
    public CharacterInBattle Defender { get; set; }
    private int TickCount { get; set; }

    private CharacterInBattle Atk => Attacker;
    private CharacterInBattle Def => Defender;

    private BattleDialog UI => GameCore.Instance.MainUI.BattleDialog;
    private bool NeedUI => Attacker.IsPlayer || Defender.IsPlayer;

    public Battle(CharacterInBattle atk, CharacterInBattle def)
    {
        Attacker = atk;
        Defender = def;
    }

    public async ValueTask<BattleResult> Do()
    {
        Debug.Log($"[戦闘処理] {Atk}) -> {Def} at {Atk.Area.Position} -> {Def.Area.Position}");
        Debug.Log($"[戦闘処理] 攻撃側地形: {Atk.Terrain} 防御側地形: {Def.Terrain}");
        if (Def.Character == null)
        {
            Debug.Log($"[戦闘処理] 防御側がいないので侵攻側の勝利です。");
            return BattleResult.AttackerWin;
        }

        if (NeedUI)
        {
            UI.Root.style.display = DisplayStyle.Flex;
            UI.SetData(this);
        }

        var result = default(BattleResult);
        while (!Atk.AllSoldiersDead && !Def.AllSoldiersDead)
        {
            // 撤退判断を行う。
            if (NeedUI)
            {
                UI.SetData(this);
                var shouldContinue = await UI.WaitPlayerClick();
                if (!shouldContinue)
                {
                    result = Atk.IsPlayer ?
                        BattleResult.DefenderWin :
                        BattleResult.AttackerWin;
                    break;
                }
            }
            if (Atk.ShouldRetreat())
            {
                result = BattleResult.DefenderWin;
                break;
            }
            if (Def.ShouldRetreat())
            {
                result = BattleResult.AttackerWin;
                break;
            }

            Tick();
        }

        if (result == BattleResult.None)
        {
            result = Atk.AllSoldiersDead ?
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
        foreach (var sol in Atk.Force.Soldiers.Concat(Def.Force.Soldiers))
        {
            if (sol.Hp == 0)
            {
                sol.IsEmptySlot = true;
            }
        }

        // 兵士の回復処理を行う。
        Atk.Recover(result == BattleResult.AttackerWin);
        Def.Recover(result == BattleResult.DefenderWin);

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
        TickCount += 1;

        // 両方の兵士をランダムな順番の配列にいれる。
        var all = Atk.Force.Soldiers.Select(s => (soldier: s, owner: Attacker))
            .Concat(Def.Force.Soldiers.Select(s => (soldier: s, owner: Defender)))
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

        if (Atk.IsPlayer || Def.IsPlayer)
        {
            Debug.Log($"[戦闘処理] " +
                $"{Atk}の総ダメージ: {attackerTotalDamage} " +
                $"{Def}の総ダメージ: {defenderTotalDamage}");
        }
    }
}
