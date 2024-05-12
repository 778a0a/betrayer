using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class BattleManager
{
    /// <summary>
    /// 戦闘を行います。
    /// </summary>
    public static async Awaitable<BattleResult> Battle(
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

        var ui = Test.Instance.MainUI.BattleDialog;
        var needUI = attacker.IsPlayer || defender.IsPlayer;
        if (needUI)
        {
            ui.Root.style.display = DisplayStyle.Flex;
            ui.SetData(attacker, attackerTerrain, defender, defenderTerrain);
        }

        var result = default(BattleResult);
        var tickCount = 0;
        while (
            attacker.Force.Soldiers.Any(s => s.IsAlive) &&
            defender.Force.Soldiers.Any(s => s.IsAlive))
        {
            if (needUI)
            {
                ui.SetData(attacker, attackerTerrain, defender, defenderTerrain);
                var shouldContinue = await ui.WaitPlayerClick();
                if (!shouldContinue)
                {
                    result = attacker.IsPlayer ? BattleResult.DefenderWin : BattleResult.AttackerWin;
                    break;
                }
            }

            Tick(attackerTerrain, defenderTerrain, attacker, defender, tickCount++);
        }

        if (result == BattleResult.None)
        {
            result = attacker.Force.Soldiers.Any(s => s.IsAlive) ? BattleResult.AttackerWin : BattleResult.DefenderWin;
        }
        Debug.Log($"[戦闘処理] 結果: {result}");
        if (needUI)
        {
            ui.SetData(attacker, attackerTerrain, defender, defenderTerrain);
            await ui.WaitPlayerClick();
            ui.Root.style.display = DisplayStyle.None;
        }

        foreach (var sol in attacker.Force.Soldiers.Concat(defender.Force.Soldiers))
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

    private static void Tick(
        Terrain attackerTerrain,
        Terrain defenderTerrain,
        Character attacker,
        Character defender,
        int tickCount)
    {
        // 両方の兵士をランダムな順番の配列にいれる。
        var all = attacker.Force.Soldiers.Select(s => (soldier: s, owner: attacker, opponent: defender))
            .Concat(defender.Force.Soldiers.Select(s => (soldier: s, owner: defender, opponent: attacker)))
            .Where(x => x.soldier.IsAlive)
            .ToArray()
            .ShuffleInPlace();

        var baseAdjustment = new Dictionary<Character, float>
        {
            {attacker, BaseAdjustment(attacker.Attack, defender.Defense, attacker.Intelligence, defender.Intelligence, defenderTerrain, tickCount)},
            {defender, BaseAdjustment(defender.Defense, attacker.Attack, defender.Intelligence, attacker.Intelligence, attackerTerrain, tickCount)},
        };
        static float BaseAdjustment(int atk, int def, int atkInt, int defInt, Terrain terrain, int tickCount)
        {
            var adj = 1f;
            adj += (atk - 50) / 100f;
            adj -= (def - 50) / 100f;
            adj += (atkInt - 50) / 100f * Mathf.Min(1, tickCount / 10f);
            adj -= (defInt - 50) / 100f * Mathf.Min(1, tickCount / 10f);
            adj += TerrainDamageAdjustment(terrain);
            return adj;
        }
        Debug.Log($"[戦闘処理] 基本調整値: atk:{baseAdjustment[attacker]:0.00} def:{baseAdjustment[defender]:0.00}");

        var attackerTotalDamage = 0f;
        var defenderTotalDamage = 0f;
        foreach (var (soldier, owner, opponent) in all)
        {
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

            if (owner == attacker)
            {
                attackerTotalDamage += damage;
            }
            else
            {
                defenderTotalDamage += damage;
            }
        }

        if (attacker.IsPlayer || defender.IsPlayer)
        {
            Debug.Log($"[戦闘処理] {attacker.Name}の総ダメージ: {attackerTotalDamage} {defender.Name}の総ダメージ: {defenderTotalDamage}");
        }
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
