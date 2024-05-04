using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;


public class StrategyActions
{
    public static void Initialize(WorldData world)
    {
        foreach (var action in All)
        {
            action.World = world;
        }
    }

    /// <summary>
    /// 勢力の序列と給料配分を調整します。
    /// </summary>
    public static OrganizeAction Organize { get; } = new();
    public class OrganizeAction : StrategyActionBase
    {
        public override int Cost(Character chara) => 0;
        protected override bool CanDoCore(Character chara) => true;

        public override void Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            var country = World.CountryOf(chara);
            // 貢献順に並び替える。
            country.Vassals = country.Vassals.OrderBy(c => c.Contribution).ToList();

            // 給料配分を調整する。
            var salaryTable = table[country.Vassals.Count];
            for (int i = 0; i < country.Vassals.Count; i++)
            {
                var vassal = country.Vassals[i];
                vassal.SalaryRatio = salaryTable[i + 1];
            }
            country.RecalculateSalary();
        }

        private static readonly int[][] table = new[]
        {
            new[] {100, },
            new[] {70, 30, },
            new[] {55, 25, 20, },
            new[] {47, 20, 18, 15, },
            new[] {40, 18, 16, 14, 12, },
            new[] {39, 16, 14, 12, 10, 9, },
            new[] {35, 14, 13, 11, 10, 9, 8, },
            new[] {30, 13, 12, 11, 10, 9, 8, 7, },
            new[] {29, 12, 11, 10, 9, 8, 7, 7, 7, },
            new[] {22, 12, 11, 10, 9, 8, 7, 7, 7, 7, },
        };
    }


    /// <summary>
    /// 配下を雇います。
    /// </summary>
    public static HireVassalAction HireVassal { get; } = new();
    public class HireVassalAction : StrategyActionBase
    {
        public override int Cost(Character chara) => 8;
        protected override bool CanDoCore(Character chara)
        {
            if (!World.Characters.Any(World.IsFree)) return false;

            // 配下の枠に空きがなければ実行不可。
            var country = World.CountryOf(chara);
            if (country.Vassals.Count >= country.VassalCountMax) return false;

            return true;
        }

        public override void Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            // ランダムに所属なしのキャラを選ぶ。
            var frees = World.Characters.Where(World.IsFree).ToList();
            var candidates = new List<Character>();
            var candCount = (int)MathF.Max(1, MathF.Ceiling(chara.Intelligence / 10) - 5);
            for (int i = 0; i < candCount; i++)
            {
                if (frees.Count == 0) break;
                var cand = frees.RandomPick();
                candidates.Add(cand);
                frees.Remove(cand);
            }

            // 配下にする。
            var country = World.CountryOf(chara);
            var newVassal = candidates.OrderByDescending(c => c.Power).First();
            country.AddVassal(newVassal);
        }
    }

    /// <summary>
    /// ランダムに隣接国に侵攻します。
    /// </summary>
    public static AttackRandomlyAction AttackRandomly { get; } = new();
    public class AttackRandomlyAction : StrategyActionBase
    {
        public override bool CanSelect(Character chara) => World.IsRuler(chara);
        public override int Cost(Character chara) => 5;
        protected override bool CanDoCore(Character chara)
        {
            var country = World.CountryOf(chara);
            // 全員行動済みならNG。
            if (country.Members.All(c => c.IsAttacked)) return false;

            // 侵攻できるエリアがないならNG。
            var neighborAreas = GetAttackableAreas(World, country);
            if (neighborAreas.Count == 0) return false;

            return true;
        }

        public override void Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            var country = World.CountryOf(chara);
            var neighborAreas = GetAttackableAreas(World, country);
            var targetArea = neighborAreas.RandomPick();
            var targetCountry = World.CountryOf(targetArea);

            var attacker = country.Members.Where(c => !c.IsAttacked).RandomPick();
            var defender = targetCountry.Members.Where(c => !c.IsAttacked).RandomPickDefault();

            // 攻撃側のエリアを決定する。一番攻撃側が有利なエリアを選ぶ。
            var sourceArea = GetAttackSourceArea(World.Map, targetArea, country);

            // 侵攻する。
            var result = BattleManager.Battle(World.Map, sourceArea, targetArea, attacker, defender);
            attacker.IsAttacked = true;
            if (result == BattleResult.AttackerWin)
            {
                attacker.Contribution += 2;
                BattleManager.Recover(attacker, true);
                if (defender != null)
                {
                    defender.Contribution += 1;
                    BattleManager.Recover(defender, false);
                }

                country.Areas.Add(targetArea);
                targetCountry.Areas.Remove(targetArea);
                // 領土がなくなったら国を削除する。
                if (targetCountry.Areas.Count == 0)
                {
                    World.Countries.Remove(targetCountry);
                    foreach (var c in World.Countries)
                    {
                        if (c.Ally == targetCountry) c.Ally = null;
                    }
                }
            }
            else
            {
                attacker.Contribution += 1;
                BattleManager.Recover(attacker, false);
                defender.Contribution += 2;
                BattleManager.Recover(defender, true);
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

        private static List<Area> GetAttackableAreas(WorldData World, Country country)
        {
            var neighborAreas = new List<Area>();
            foreach (var area in country.Areas)
            {
                var neighbors = World.Map.GetNeighbors(area);
                foreach (var neighbor in neighbors)
                {
                    // 自国か同盟国ならスキップする。
                    var owner = World.CountryOf(neighbor);
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
    public static FireVassalAction FireVassal { get; } = new();
    public class FireVassalAction : StrategyActionBase
    {
        public override int Cost(Character chara) => 1;
        protected override bool CanDoCore(Character chara)
        {
            var country = World.CountryOf(chara);
            return country.Vassals.Count > 0;
        }

        public override void Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            var country = World.CountryOf(chara);
            var target = country.Vassals.OrderBy(c => c.Power).First();
            country.Vassals.Remove(target);
        }
    }

    /// <summary>
    /// 同盟を結びます。
    /// </summary>
    public static AllyAction Ally { get; } = new();
    public class AllyAction : StrategyActionBase
    {
        public override int Cost(Character chara) => 5;
        protected override bool CanDoCore(Character chara)
        {
            var country = World.CountryOf(chara);
            return World.Countries.Count > 2 &&
                country.Ally == null &&
                World.Countries.Except(new[] { country }).Any(c => c.Ally == null);
        }

        public override void Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            var country = World.CountryOf(chara);
            var neighbors = World.Neighbors(country);

            // 隣接国が1つしかない場合は、その国以外と同盟を結ぶ。
            var target = default(Country);
            if (neighbors.Length == 1)
            {
                var cands = World.Countries
                    .Except(new[] { country })
                    .Except(neighbors)
                    .Where(c => c.Ally == null)
                    .ToList();
                target = cands.RandomPick();
            }
            // 隣接国が複数ある場合は、その中からランダムに選ぶ。
            else
            {
                target = neighbors.RandomPick();
            }

            if (Random.value < 0.5)
            {
                country.Ally = target;
                target.Ally = country;
                Debug.Log($"{country} と {target} が同盟を結びました。");
            }
            else
            {
                Debug.Log($"{country} が {target} に同盟を申し込みましたが、拒否されました。");
            }
        }
    }

    /// <summary>
    /// 勢力を捨てて放浪します。
    /// </summary>
    public static ResignAction Resign { get; } = new();
    public class ResignAction : StrategyActionBase
    {
        public override int Cost(Character chara) => 1;
        protected override bool CanDoCore(Character chara)
        {
            var country = World.CountryOf(chara);
            return country.Vassals.Count > 0;
        }

        public override void Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            var country = World.CountryOf(chara);
            var successor = country.Vassals[0];

            country.Vassals.RemoveAt(0);
            country.Ruler = successor;
            country.RecalculateSalary();
            Debug.Log($"{chara.Name} が勢力を捨てて、{successor.Name} が新たな君主となりました。");
        }
    }


    public static StrategyActionBase[] All { get; } = new StrategyActionBase[]
    {
        Organize,
        HireVassal,
        FireVassal,
        Ally,
        Resign,
        AttackRandomly,
    };
}

public class StrategyActionBase
{
    public WorldData World { get; set; }

    /// <summary>
    /// 選択肢として表示可能ならtrue
    /// </summary>
    public virtual bool CanSelect(Character chara) => World.IsRuler(chara);
    /// <summary>
    /// アクションの実行に必要なGold
    /// </summary>
    public virtual int Cost(Character chara) => 0;
    /// <summary>
    /// アクションを実行可能ならtrue
    /// </summary>
    public bool CanDo(Character chara) =>
        CanSelect(chara) &&
        chara.Gold >= Cost(chara) &&
        CanDoCore(chara);
    /// <summary>
    /// アクションを実行可能ならtrue（子クラスでのオーバーライド用）
    /// </summary>
    /// <returns></returns>
    protected virtual bool CanDoCore(Character chara) => true;
    /// <summary>
    /// アクションを実行します。
    /// </summary>
    public virtual void Do(Character chara) { }
}
