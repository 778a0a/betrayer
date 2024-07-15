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
    public ProvokeAction Provoke { get; } = new();
    public class ProvokeAction : MartialActionBase
    {
        public override string Description => L["他国の将を挑発します。"];

        public override bool CanSelect(Character chara) => World.IsRulerOrVassal(chara);
        public override int Cost(Character chara) => 8;
        protected override bool CanDoCore(Character chara)
        {
            var country = World.CountryOf(chara);
            var neighbors = World.Neighbors(country).Where(c => c.Ally != country);
            return neighbors.Any();
        }

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));

            var country = World.CountryOf(chara);
            var neighborAreas = World.GetAttackableAreas(country);

            var attacker = default(Character);
            if (chara.IsPlayer)
            {
                var neighborCountries = World.Neighbors(country).Where(c => c.Ally != country).ToArray();
                attacker = await UI.ShowSelectProvokingTargetScreen(
                    neighborCountries,
                    World);
                if (attacker == null)
                {
                    Debug.Log("キャンセルされました。");
                    return;
                }
            }
            else
            {
                var randArea = neighborAreas.RandomPick();
                var randCountry = World.CountryOf(randArea);
                attacker = randCountry.Members.RandomPickDefault();
            }

            var chance = 0.01f * Mathf.Max(1, 10 * (chara.Intelligence - attacker.Intelligence));
            if (chance.Chance())
            {
                Debug.Log("挑発に成功しました。");
                if (chara.IsPlayer)
                {
                    await MessageWindow.Show(L["挑発に成功しました。"]);
                }
            }
            else
            {
                if (chara.IsPlayer)
                {
                    await MessageWindow.Show(L["挑発は失敗しました。"]);
                    PayCost(chara);
                    return;
                }
            }
            if (attacker.IsPlayer)
            {
                await MessageWindow.Show(L["{0}に挑発されました。", chara.Name]);
            }

            var sourceCountry = World.CountryOf(attacker);
            var sourceArea = neighborAreas.Where(sourceCountry.Areas.Contains).RandomPick();

            // 防衛側のエリアを決定する。一番防衛側が有利なエリアを選ぶ。
            var targetArea = GetDefenceArea(World.Map, sourceArea, country);
            var defender = chara;

            var sourcePosition = sourceArea.Position;
            var targetDirection = sourcePosition.GetDirectionTo(targetArea.Position);
            Core.Tilemap.ShowAttackDirectionArrow(sourceArea.Position, targetDirection);
            using var _ = Util.Defer(() => Core.Tilemap.HideAttackDirectionArrow());

            // 侵攻する。
            var battle = BattleManager.Prepare(sourceArea, targetArea, attacker, defender, this);
            var result = await battle.Do();
            AttackAction.OnAfterAttack(battle, result, World);
            // 攻撃済みフラグはつけない。

            PayCost(chara);
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
                    return Battle.TerrainDamageAdjustment(terrain);
                })
                .First();
        }
    }
}

