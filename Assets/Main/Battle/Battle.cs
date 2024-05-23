﻿using System;
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