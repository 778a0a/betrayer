using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;


public class MartialActions
{
    public static void Initialize(WorldData world)
    {
        foreach (var action in All)
        {
            action.World = world;
        }
    }

    /// <summary>
    /// 隣接国に侵攻します。
    /// </summary>
    public static AttackAction Attack { get; } = new();
    public class AttackAction : MartialActionBase
    {
        public override bool CanSelect(Character chara) => World.IsRuler(chara);
        public override int Cost(Character chara) => 3;
        protected override bool CanDoCore(Character chara)
        {
            var country = World.CountryOf(chara);
            // 全員行動済みならNG。
            if (country.Members.All(c => c.IsAttacked)) return false;

            // 侵攻できるエリアがないならNG。
            var neighborAreas = World.GetAttackableAreas(country);
            if (neighborAreas.Count == 0) return false;

            return true;
        }

        public override async void Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara); // TODO

            var country = World.CountryOf(chara);
            var neighborAreas = World.GetAttackableAreas(country);
            var targetArea = neighborAreas.RandomPick();
            var targetCountry = World.CountryOf(targetArea);


            var attacker = country.Members.Where(c => !c.IsAttacked).RandomPick();
            if (chara.IsPlayer)
            {
                attacker = await Test.Instance.MainUI.ShowSelectAttackerScreen(country, World);
                if (attacker == null)
                {
                    Test.Instance.MainUI.ShowMartialUI();
                    return;
                }
            }

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
                Test.Instance.tilemap.DrawCountryTile(World);
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

            if (chara.IsPlayer)
            {
                Test.Instance.MainUI.ShowMartialUI();
                Test.Instance.MainUI.MartialPhase.SetData(chara, World);
                Test.Instance.tilemap.DrawCountryTile(World);
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
    }

    /// <summary>
    /// 挑発
    /// </summary>
    public static ProvokeAction Provoke { get; } = new();
    public class ProvokeAction : MartialActionBase
    {
        public override bool CanSelect(Character chara) => World.IsRulerOrVassal(chara);
        public override int Cost(Character chara) => 8;
        protected override bool CanDoCore(Character chara)
        {
            var country = World.CountryOf(chara);
            var neighbors = World.Neighbors(country).Where(c => c.Ally != country);
            return neighbors.Any();
        }

        public override void Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            var country = World.CountryOf(chara);
            var neighborAreas = World.GetAttackableAreas(country);
            var sourceArea = neighborAreas.RandomPick();
            var sourceCountry = World.CountryOf(sourceArea);

            var attacker = sourceCountry.Members.RandomPickDefault();
            var defender = chara;

            // 防衛側のエリアを決定する。一番防衛側が有利なエリアを選ぶ。
            var targetArea = GetDefenceArea(World.Map, sourceArea, country);

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

                sourceCountry.Areas.Add(targetArea);
                country.Areas.Remove(targetArea);
                // 領土がなくなったら国を削除する。
                if (country.Areas.Count == 0)
                {
                    World.Countries.Remove(country);
                    foreach (var c in World.Countries)
                    {
                        if (c.Ally == country) c.Ally = null;
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
        private static Area GetDefenceArea(MapGrid map, Area source, Country defenceCountry)
        {
            return map
                // 攻撃対象の隣接エリアを取得する。
                .GetNeighbors(source)
                // 自国のエリアのみに絞り込む。
                .Where(a => defenceCountry.Areas.Contains(a))
                // もっとも防衛側が有利なエリアを取得する。
                .OrderBy(a =>
                {
                    //var dir = source.GetDirectionTo(a);
                    //var attackerTerrain = map.Helper.GetAttackerTerrain(a.Position, dir);
                    //var adj = BattleManager.TerrainDamageAdjustment(attackerTerrain);
                    var terrain = map.Helper.GetTerrain(a.Position);
                    return BattleManager.TerrainDamageAdjustment(terrain);
                })
                .First();
        }
    }

    /// <summary>
    /// 討伐
    /// </summary>
    public static SubdueAction Subdue { get; } = new();
    public class SubdueAction : MartialActionBase
    {
        public override bool CanSelect(Character chara) => World.IsRuler(chara);
        public override int Cost(Character chara) => 5;
        protected override bool CanDoCore(Character chara) =>
            !chara.IsAttacked &&
            World.CountryOf(chara).Vassals.Count > 0;

        public override async void Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara); // TODO

            var country = World.CountryOf(chara);

            // 戦う場所を決める。
            var (attack, defend) = country.Areas
                .SelectMany(a => World.Map.GetNeighbors(a)
                    .Where(country.Areas.Contains)
                    .Select(x => (a, x)))
                .DefaultIfEmpty((country.Areas[0], country.Areas[0]))
                .RandomPickDefault();

            if (chara.IsPlayer)
            {
                var target = await Test.Instance.MainUI.ShowSubdueScreen(country, World);
                if (target == null)
                {
                    Test.Instance.MainUI.ShowMartialUI();
                    return;
                }

                // 攻撃する。
                var result = BattleManager.Battle(World.Map, attack, defend, chara, target);
                chara.IsAttacked = true;
                if (result == BattleResult.AttackerWin)
                {
                    BattleManager.Recover(chara, true);
                    BattleManager.Recover(target, false);
                    country.Vassals.Remove(target);
                }
                else
                {
                    BattleManager.Recover(chara, false);
                    BattleManager.Recover(target, true);
                }
                Test.Instance.MainUI.ShowMartialUI();
                Test.Instance.MainUI.MartialPhase.SetData(chara, World);
            }
            else
            {
                // 一番忠誠の低い配下を選ぶ。
                var target = country.Vassals.OrderBy(c => c.Loyalty).First();

                // 攻撃する。
                var result = BattleManager.Battle(World.Map, attack, defend, chara, target);
                chara.IsAttacked = true;
                if (result == BattleResult.AttackerWin)
                {
                    BattleManager.Recover(chara, true);
                    BattleManager.Recover(target, false);
                    country.Vassals.Remove(target);
                }
                else
                {
                    BattleManager.Recover(chara, false);
                    BattleManager.Recover(target, true);
                }
            }
        }
    }

    /// <summary>
    /// 私闘
    /// </summary>
    public static PrivateFightAction PrivateFight { get; } = new();
    public class PrivateFightAction : MartialActionBase
    {
        public override bool CanSelect(Character chara) => World.IsVassal(chara);
        public override int Cost(Character chara) => 5;
        protected override bool CanDoCore(Character chara) =>
            !chara.IsAttacked &&
            World.CountryOf(chara).Vassals.Count > 1;

        public override void Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            var country = World.CountryOf(chara);

            var target = country.Vassals.Where(v => v != chara).RandomPick();

            // 戦う場所を決める。
            var (attack, defend) = country.Areas
                .SelectMany(a => World.Map.GetNeighbors(a)
                    .Where(country.Areas.Contains)
                    .Select(x => (a, x)))
                .DefaultIfEmpty((country.Areas[0], country.Areas[0]))
                .RandomPickDefault();

            // 攻撃する。
            var result = BattleManager.Battle(World.Map, attack, defend, chara, target);
            chara.IsAttacked = true;
            if (result == BattleResult.AttackerWin)
            {
                BattleManager.Recover(chara, true);
                BattleManager.Recover(target, false);
            }
            else
            {
                BattleManager.Recover(chara, false);
                BattleManager.Recover(target, true);
            }
        }
    }

    public static MartialActionBase[] All { get; } = new MartialActionBase[]
    {
        Attack,
        //GreatWar,
        Provoke,
        Subdue,
        PrivateFight,
    };
}

public class MartialActionBase
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
