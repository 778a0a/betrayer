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
        private readonly Dictionary<Character, (Country, Area)> contexts = new();

        public override string Description => "隣接国に侵攻します。";

        public override bool CanSelect(Character chara) => World.IsRulerOrVassal(chara);
        public override int Cost(Character chara) => 3;
        protected override bool CanDoCore(Character chara)
        {
            var country = World.CountryOf(chara);

            // 自身が配下で侵攻済みの場合はNG。
            if (World.IsVassal(chara) && chara.IsAttacked) return false;

            // 全員行動済みならNG。
            if (country.Members.All(c => c.IsAttacked)) return false;

            // 侵攻できるエリアがないならNG。
            var neighborAreas = World.GetAttackableAreas(country);
            if (neighborAreas.Count == 0) return false;

            return true;
        }

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));

            var country = World.CountryOf(chara);

            // 攻撃先エリアを選択する。
            var neighborAreas = World.GetAttackableAreas(country);
            // 前回攻撃したエリア、国を継続して攻撃しやすくする。
            if (0.5.Chance() && contexts.TryGetValue(chara, out var context))
            {
                var prevCountry = context.Item1;
                var prevArea = context.Item2;
                if (0.5.Chance() && neighborAreas.Contains(prevArea))
                {
                    neighborAreas = new() { prevArea };
                }
                else
                {
                    var prevCountryAreas = neighborAreas
                        .Where(a => World.CountryOf(a) == prevCountry)
                        .ToList();
                    if (prevCountryAreas.Count > 0) neighborAreas = prevCountryAreas;
                }
            }

            var targetArea = neighborAreas.RandomPick();
            if (0.5.Chance())
            {
                // 有利なエリアを攻撃しやすくする。
                targetArea = neighborAreas
                    .Select(a => new
                    {
                        a,
                        // 敵の地形に対する自分の優位さ。高いほど自分が優位。基本的に0以下の値。
                        defAdj = Battle.TerrainDamageAdjustment(a.Terrain),
                        // 敵へ攻め込むときの自身の地形の優位さ。高いほど自分が優位。0.5～-0.5程度
                        atkAdk = -Battle.TerrainDamageAdjustment(
                            World.Map.Helper.GetAttackerTerrain(
                                GetAttackSourceArea(World.Map, targetArea, country).Position,
                                GetAttackSourceArea(World.Map, targetArea, country).GetDirectionTo(a)))
                    })
                    .RandomPickWeighted(x => 2 + x.defAdj + x.atkAdk)
                    .a;
            }

            var targetCountry = World.CountryOf(targetArea);
            // プレーヤーの場合は攻め込むエリアを選択させる。
            if (chara.IsPlayer)
            {
                targetArea = await UI.ShowSelectAreaScreen(neighborAreas, World);
                if (targetArea == null)
                {
                    Debug.Log("キャンセルされました。");
                    return;
                }
                targetCountry = World.CountryOf(targetArea);
            }
            // 一番有利なエリアから攻撃する。
            var sourceArea = GetAttackSourceArea(World.Map, targetArea, country);

            var sourcePosition = sourceArea.Position;
            var targetDirection = sourcePosition.GetDirectionTo(targetArea.Position);
            Core.Tilemap.ShowAttackDirectionArrow(sourceArea.Position, targetDirection);
            using var _ = Util.Defer(() => Core.Tilemap.HideAttackDirectionArrow());

            // 攻撃側キャラを選択する。
            var attacker = default(Character);
            if (chara.IsPlayer)
            {
                // プレーヤーで君主の場合は攻撃者を選択させる。
                if (country.Ruler == chara)
                {
                    attacker = await UI.ShowSelectAttackerScreen(country, World);
                    if (attacker == null)
                    {
                        Debug.Log("キャンセルされました。");
                        return;
                    }
                }
                // 配下の場合は本当に侵攻するか確認する。
                else
                {
                    attacker = chara;
                }
            }
            else
            {
                var attackerCands = country.Members
                    .Where(c => !c.IsAttacked)
                    .Where(c => c.Power > 500);
                attacker = attackerCands.RandomPickDefault();
                if (0.5.Chance())
                {
                    attacker = attackerCands
                        // 強いキャラが防衛に回りにくくなるので却下。
                        //.RandomPickWeighted(c => c.Power * Mathf.Sqrt(c.Attack));
                        // 攻撃が得意なキャラが選ばれやすくしてみる。
                        .RandomPickWeighted(c => (float)c.Attack / c.Defense);
                }
                if (attacker.IsPlayer)
                {
                    await MessageWindow.Show("出撃命令が下りました。");
                }
            }

            // 防衛側キャラを選択する。
            var defender = await SelectDefender(
                attacker,
                sourceArea,
                country,
                targetArea,
                targetCountry);

            // 侵攻する。
            var battle = BattleManager.Prepare(sourceArea, targetArea, attacker, defender, this);
            var result = await battle.Do();
            OnAfterAttack(battle, result, World);

            // 攻撃済みフラグを立てる。
            attacker.IsAttacked = true;

            if (targetCountry.Ruler.IsPlayer)
            {
                UI.MartialPhase.SetData(chara, World);
                UI.ShowMartialUI();
                Util.Todo();
            }

            contexts[chara] = (country, targetArea);

            PayCost(chara);
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
                    var adj = Battle.TerrainDamageAdjustment(attackerTerrain);
                    return adj;
                })
                .First();
        }

        public async ValueTask<Character> SelectDefender(
            Character attacker,
            Area sourceArea,
            Country country,
            Area targetArea,
            Country targetCountry)
        {
            // 防衛側キャラを選択する。
            var candidates = targetCountry.Members.Where(c => !c.IsAttacked);
            var defender = Random.value switch
            {
                <= 0.20f => candidates.RandomPickWeighted(c => c.Power),
                <= 0.8f => candidates.RandomPickWeighted(c =>
                    c.Force.Soldiers.Where(s => s.IsAlive).Select(s => s.HpFloat).DefaultIfEmpty(0.001f).Min()
                    *
                    (c.Defense * 2 - c.Attack)),
                _ => candidates.RandomPickWeighted(c =>
                    Mathf.Pow(c.Force.Soldiers.Where(s => s.IsAlive).Select(s => s.HpFloat).DefaultIfEmpty(0.001f).Min(), 2)),
            };


            // 最後のエリアの場合は、君主は行動済みでも防衛可能にする。
            // 固定で一番強いキャラを選ぶ。
            if (targetCountry.Areas.Count == 1)
            {
                defender = targetCountry.Members
                    .Where(c => !c.IsAttacked || c == targetCountry.Ruler)
                    .OrderByDescending(c => c.Power)
                    .First();
            }

            // 防衛側の君主がプレーヤーの場合は防衛者を選択させる。
            if (targetCountry.Ruler.IsPlayer)
            {
                defender = await UI.ShowSelectDefenderScreen(
                    sourceArea,
                    country,
                    attacker,
                    targetArea,
                    targetCountry,
                    World);
            }
            else if (defender.IsPlayer)
            {
                await MessageWindow.Show("防衛命令が下りました。");
            }

            return defender;
        }

        /// <summary>
        /// 通常の侵攻戦闘後の処理
        /// </summary>
        public static void OnAfterAttack(Battle battle, BattleResult result, WorldData world)
        {
            var atk = battle.Attacker;
            var def = battle.Defender;

            // 攻撃側の勝ち
            if (result == BattleResult.AttackerWin)
            {
                atk.Character.Contribution += 10;
                if (def.Character != null)
                {
                    def.Character.Contribution += 5;
                }

                // 領土を更新する。
                var area = def.Area;
                atk.Country.Areas.Add(area);
                def.Country.Areas.Remove(area);
                GameCore.Instance.Tilemap.DrawCountryTile();
                GameCore.Instance.Tilemap.SetExhausted(area, atk.Country.IsExhausted);

                // 領土がなくなったら国を削除する。
                if (def.Country.Areas.Count == 0)
                {
                    world.Countries.Remove(def.Country);
                    foreach (var c in world.Countries)
                    {
                        if (c.Ally == def.Country) c.Ally = null;
                    }
                }
            }
            // 防衛側の勝ち
            else
            {
                atk.Character.Contribution += 1;
                def.Character.Contribution += 10;
            }
        }
    }
}

