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
    /// 隣接国に侵攻します。
    /// </summary>
    public AttackAction Attack { get; } = new();
    public class AttackAction : MartialActionBase
    {
        public override bool CanSelect(Character chara) => World.IsRulerOrVassal(chara);
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

        public override async Awaitable Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara); // TODO

            var country = World.CountryOf(chara);

            // 攻撃先エリアを選択する。
            var neighborAreas = World.GetAttackableAreas(country);
            var targetArea = neighborAreas.RandomPick();
            var targetCountry = World.CountryOf(targetArea);
            // プレーヤーの場合は攻め込むエリアを選択させる。
            if (chara.IsPlayer)
            {
                targetArea = await Test.Instance.MainUI.ShowSelectAreaScreen(neighborAreas, World);
                if (targetArea == null)
                {
                    Test.Instance.MainUI.ShowMartialUI();
                    return;
                }
                targetCountry = World.CountryOf(targetArea);
            }
            // 一番有利なエリアから攻撃する。
            var sourceArea = GetAttackSourceArea(World.Map, targetArea, country);

            // 攻撃側キャラを選択する。
            var attacker = country.Members.Where(c => !c.IsAttacked).RandomPick();
            // プレーヤーの場合は攻撃者を選択させる。
            if (chara.IsPlayer)
            {
                attacker = await Test.Instance.MainUI.ShowSelectAttackerScreen(country, World);
                if (attacker == null)
                {
                    Test.Instance.MainUI.ShowMartialUI();
                    return;
                }
            }

            // 防衛側キャラを選択する。
            var defender = targetCountry.Members.Where(c => !c.IsAttacked).RandomPickDefault();
            // 防衛側の君主がプレーヤーの場合は防衛者を選択させる。
            if (targetCountry.Ruler.IsPlayer)
            {
                defender = await Test.Instance.MainUI.ShowSelectDefenderScreen(
                    sourceArea,
                    country,
                    attacker,
                    targetArea,
                    targetCountry,
                    World);
            }

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

            if (chara.IsPlayer || targetCountry.Ruler.IsPlayer)
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
}

