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
using Random = UnityEngine.Random;

public class BattleManager
{
    /// <summary>
    /// 戦闘を行います。
    /// </summary>
    public static BattleResult Battle(MapGrid map, Area sourceArea, Area targetArea, Character attacker, Character defender)
    {
        var dir = sourceArea.GetDirectionTo(targetArea);
        var attackerTerrain = map.Helper.GetAttackerTerrain(sourceArea.Position, dir);
        var defenderTerrain = targetArea.Terrain;
        Debug.Log($"[戦闘処理] {attacker.Name}({attacker.Power}) -> {defender.Name}({defender.Power}) at {sourceArea.Position} -> {targetArea.Position}");
        Debug.Log($"[戦闘処理] 攻撃側地形: {attackerTerrain} 防御側地形: {defenderTerrain}");
        if (defender == null)
        {
            Debug.Log($"[戦闘処理] 防御側がいないので侵攻側の勝利です。");
            return BattleResult.AttackerWin;
        }

        while (attacker.Force.Soldiers.Any(s => s.IsAlive) && defender.Force.Soldiers.Any(s => s.IsAlive))
        {
            Tick(attackerTerrain, defenderTerrain, attacker, defender);
        }
        var result = attacker.Force.Soldiers.Any(s => s.IsAlive) ? BattleResult.AttackerWin : BattleResult.DefenderWin;
        Debug.Log($"[戦闘処理] 結果: {result}");
        return result;
    }

    
    public static float TerrainDamageAdjustment(Terrain t) => t switch
    {
        Terrain.LargeRiver => 1.40f,
        Terrain.River => 1.25f,
        Terrain.Plain => 1.0f,
        Terrain.Hill => 0.9f,
        Terrain.Forest => 0.75f,
        Terrain.Mountain => 0.60f,
        Terrain.Fort => 0.5f,
        _ => 1.0f
    };

    private static void Tick(Terrain attackerTerrain, Terrain defenderTerrain, Character attacker, Character defender)
    {
        // 両方の兵士をランダムな順番の配列にいれる。
        var all = attacker.Force.Soldiers.Select(s => (soldier: s, owner: attacker, opponent: defender, terrain: defenderTerrain, atk: attacker.Attack, def: defender.Defense))
            .Concat(defender.Force.Soldiers.Select(s => (soldier: s, owner: defender, opponent: attacker, terrain: attackerTerrain, atk: defender.Defense, def: attacker.Attack)))
            .Where(x => x.soldier.IsAlive)
            .OrderBy(_ => Random.value)
            .ToArray();

        foreach (var (soldier, owner, opponent, terrain, atk, def) in all)
        {
            if (!soldier.IsAlive) continue;
            var target = opponent.Force.Soldiers.Where(s => s.IsAlive).RandomPickDefault();
            if (target == null) continue;
            var adj =
                (soldier.Level + atk / 10f) *
                (1 - (def - 50) / 100f) *
                Random.Range(0.8f, 1.2f) *
                TerrainDamageAdjustment(terrain);

            var damage = Math.Min(1, soldier.Level * adj);
            target.Hp = (int)Math.Max(0, target.Hp - damage);
            if (target.Hp == 0)
            {
                target.IsEmptySlot = true;
            }
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
