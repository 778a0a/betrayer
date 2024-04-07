using System;
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
    public static BattleResult Battle(Area targetArea, Character attacker, Character defender)
    {
        Debug.Log($"[戦闘処理] {attacker.Name} -> {defender.Name} at {targetArea.Position}");
        if (defender == null)
        {
            Debug.Log($"[戦闘処理] 防御側がいないので侵攻側の勝利です。");
            return BattleResult.AttackerWin;
        }

        while (attacker.Force.Soldiers.Any(s => s.IsAlive) && defender.Force.Soldiers.Any(s => s.IsAlive))
        {
            Tick(targetArea, attacker, defender);
        }
        var result = attacker.Force.Soldiers.Any(s => s.IsAlive) ? BattleResult.AttackerWin : BattleResult.DefenderWin;
        Debug.Log($"[戦闘処理] 結果: {result}");
        return result;
    }

    
    public static float TerrainDamageAdjustment(Terrain t) => t switch
    {
        Terrain.LargeRiver => 1.30f,
        Terrain.River => 1.25f,
        Terrain.Plain => 1.0f,
        Terrain.Hill => 0.9f,
        Terrain.Forest => 0.75f,
        Terrain.Mountain => 0.60f,
        Terrain.Fort => 0.5f,
        _ => 1.0f
    };

    private static void Tick(Area targetArea, Character attacker, Character defender)
    {
        foreach (var sol in attacker.Force.Soldiers)
        {
            if (!sol.IsAlive) continue;
            var target = defender.Force.Soldiers.Where(s => s.IsAlive).RandomPickDefault();
            if (target == null) continue;

            var adj =
                (attacker.Attack / 100f) *
                (1 - defender.Defense / 100f) *
                Random.Range(0.8f, 1.2f) *
                TerrainDamageAdjustment(targetArea.DefenseSideTerrain);

            var damage = Math.Min(1, sol.Level * adj);
            target.Hp = (int)Math.Max(0, target.Hp - damage);
            if (target.Hp == 0)
            {
                target.IsEmptySlot = true;
            }
        }

        foreach (var sol in defender.Force.Soldiers)
        {
            if (!sol.IsAlive) continue;
            var target = attacker.Force.Soldiers.Where(s => s.IsAlive).RandomPickDefault();
            if (target == null) continue;

            var adj =
                (Math.Max(defender.Attack, defender.Defense) / 100f) *
                (1 - attacker.Defense / 100f) *
                Random.Range(0.8f, 1.2f) *
                TerrainDamageAdjustment(targetArea.AttackSideTerrain);

            var damage = Math.Min(1, sol.Level * adj);
            target.Hp = (int)Math.Max(0, target.Hp - damage);
            if (target.Hp == 0)
            {
                target.IsEmptySlot = true;
            }
        }
    }
}
