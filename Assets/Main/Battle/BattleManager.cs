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


