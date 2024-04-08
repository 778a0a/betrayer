using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;


public class StrategyActions
{
    /// <summary>
    /// ランダムに配下を雇います。
    /// </summary>
    public static HireVassalRandomlyAction HireVassalRandomly { get; } = new();
    public class HireVassalRandomlyAction : StrategyActionBase
    {
        public override int Cost(Character chara, WorldData world) => 8;
        public override bool CanDo(Character chara, WorldData world)
        {
            if (chara.Gold < Cost(chara, world)) return false;
            if (!world.IsRuler(chara)) return false;
            if (!world.Characters.Any(world.IsFree)) return false;

            // 配下の枠に空きがなければ実行不可。
            var country = world.CountryOf(chara);
            if (country.Vassals.Count >= country.VassalCountMax) return false;

            return true;
        }
            //chara.Gold >= Cost(chara, world) &&
            //world.IsRuler(chara) &&
            //world.Characters.Any(world.IsFree) &&

        public override void Do(Character chara, WorldData world)
        {
            Assert.IsTrue(CanDo(chara, world));
            chara.Gold -= Cost(chara, world);

            // ランダムに所属なしのキャラを選ぶ。
            var target = world.Characters
                .Where(world.IsFree)
                .RandomPick();

            // 配下にする。
            var country = world.CountryOf(chara);
            var existingMembers = country.Members;
            country.Vassals.Add(target);
            target.Contribution = 0;

            // 給料配分を設定する。
            const int MinimumRatio = 10;
            target.SalaryRatio = MinimumRatio;
            // 既存の配下の配分を調整する。
            var remainingRatio = target.SalaryRatio;
            while (remainingRatio > 0)
            {
                foreach (var member in existingMembers)
                {
                    if (member.SalaryRatio <= MinimumRatio) continue;
                    member.SalaryRatio--;
                    remainingRatio--;
                    if (remainingRatio <= 0) break;
                }
            }
            chara.SalaryRatio = 100 - country.Vassals.Sum(v => v.SalaryRatio);
        }
    }

    /// <summary>
    /// ランダムに隣接国に侵攻します。
    /// </summary>
    public static AttackRandomlyAction AttackRandomly { get; } = new();
    public class AttackRandomlyAction : StrategyActionBase
    {
        public override int Cost(Character chara, WorldData world) => 5;
        public override bool CanDo(Character chara, WorldData world)
        {
            if (chara.Gold < Cost(chara, world)) return false;
            if (!world.IsRuler(chara)) return false;

            var country = world.CountryOf(chara);
            // 全員行動済みならNG。
            if (country.Members.All(c => c.IsAttacked)) return false;

            // 侵攻できるエリアがないならNG。
            var neighborAreas = GetAttackableAreas(world, country);
            if (neighborAreas.Count == 0) return false;

            return base.CanDo(chara, world);
        }

        public override void Do(Character chara, WorldData world)
        {
            Assert.IsTrue(CanDo(chara, world));
            chara.Gold -= Cost(chara, world);

            var country = world.CountryOf(chara);
            var neighborAreas = GetAttackableAreas(world, country);
            var targetArea = neighborAreas.RandomPick();
            var targetCountry = world.CountryOf(targetArea);

            var attacker = country.Members.Where(c => !c.IsAttacked).RandomPick();
            var defender = targetCountry.Members.Where(c => !c.IsAttacked).RandomPickDefault();

            // 攻撃側のエリアを決定する。一番攻撃側が有利なエリアを選ぶ。
            var sourceArea = GetAttackSourceArea(world.Map, targetArea, country);

            // 侵攻する。
            var result = BattleManager.Battle(world.Map, sourceArea, targetArea, attacker, defender);
            attacker.IsAttacked = true;
            if (result == BattleResult.AttackerWin)
            {
                attacker.Contribution += 2;
                if (defender != null) defender.Contribution += 1;
                country.Areas.Add(targetArea);
                targetCountry.Areas.Remove(targetArea);
                // 領土がなくなったら国を削除する。
                if (targetCountry.Areas.Count == 0)
                {
                    world.Countries.Remove(targetCountry);
                }
            }
            else
            {
                attacker.Contribution += 1;
                defender.Contribution += 2;
            }
        }

        /// <summary>
        /// もっとも攻撃側が有利なエリアを取得します。
        /// </summary>
        /// <returns></returns>
        private static Area GetAttackSourceArea(MapGrid map, Area target, Country attackCountry)
        {
            return map
                // 攻撃対象の隣接エリアを取得する。
                .GetNeighbors(target)
                // 自国のエリアのみに絞り込む。
                .Where(a => attackCountry.Areas.Contains(a))
                // もっとも攻撃側が有利なエリアを取得する。
                .OrderBy(a =>
                {
                    var dir = a.GetDirectionTo(target);
                    var attackerTerrain = map.Helper.GetAttackerTerrain(a.Position, dir);
                    var adj = BattleManager.TerrainDamageAdjustment(attackerTerrain);
                    return adj;
                })
                .First();
        }

        private static List<Area> GetAttackableAreas(WorldData world, Country country)
        {
            var neighborAreas = new List<Area>();
            foreach (var area in country.Areas)
            {
                var neighbors = world.Map.GetNeighbors(area);
                foreach (var neighbor in neighbors)
                {
                    // 自国か同盟国ならスキップする。
                    var owner = world.CountryOf(neighbor);
                    if (owner == country || owner == country.Ally) continue;
                    neighborAreas.Add(neighbor);
                }
            }

            return neighborAreas;
        }
    }

    /// <summary>
    /// 配下を解雇します。
    /// </summary>
    public static FireVassalMostWeakAction FireVassalMostWeak { get; } = new();
    public class FireVassalMostWeakAction : StrategyActionBase
    {
        public override int Cost(Character chara, WorldData world) => 1;
        public override bool CanDo(Character chara, WorldData world)
        {
            if (!world.IsRuler(chara)) return false;
            var country = world.CountryOf(chara);
            return country.Vassals.Count > 0;
        }

        public override void Do(Character chara, WorldData world)
        {
            Assert.IsTrue(CanDo(chara, world));
            chara.Gold -= Cost(chara, world);

            var country = world.CountryOf(chara);
            var target = country.Vassals.OrderBy(c => c.Power).First();
            country.Vassals.Remove(target);
        }
    }

    // 懲罰攻撃する
    // 同盟を結ぶ
}

public class StrategyActionBase
{
    public virtual int Cost(Character chara, WorldData world) => 0;
    public virtual bool CanDo(Character chara, WorldData world) => chara.Gold >= Cost(chara, world);
    public virtual void Do(Character chara, WorldData world) { }
}
