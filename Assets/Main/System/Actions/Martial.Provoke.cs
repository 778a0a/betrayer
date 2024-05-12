using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;


partial class MartialActions
{
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

        public override async Awaitable Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            var country = World.CountryOf(chara);
            var neighborAreas = World.GetAttackableAreas(country);

            var attacker = default(Character);
            if (chara.IsPlayer)
            {
                var neighborCountries = World.Neighbors(country).Where(c => c.Ally != country).ToArray();
                attacker = await Test.Instance.MainUI.ShowSelectProvokingTargetScreen(
                    neighborCountries,
                    World);
                if (attacker == null)
                {
                    Test.Instance.MainUI.ShowMartialUI();
                    return;
                }
            }
            else
            {
                var randArea = neighborAreas.RandomPick();
                var randCountry = World.CountryOf(randArea);
                attacker = randCountry.Members.RandomPickDefault();
            }

            var sourceCountry = World.CountryOf(attacker);
            var sourceArea = neighborAreas.Where(sourceCountry.Areas.Contains).RandomPick();

            // 防衛側のエリアを決定する。一番防衛側が有利なエリアを選ぶ。
            var targetArea = GetDefenceArea(World.Map, sourceArea, country);
            var defender = chara;

            // 侵攻する。
            var result = await BattleManager.Battle(World.Map, sourceArea, targetArea, attacker, defender);
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
                Test.Instance.tilemap.DrawCountryTile(World);
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
}

