using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

partial class PersonalActions
{
    /// <summary>
    /// エリアを攻撃して奪取します。
    /// </summary>
    public SeizeAction Seize { get; } = new();
    public class SeizeAction : PersonalActionBase
    {
        public override bool CanSelect(Character chara) => World.IsFree(chara);
        public override int Cost(Character chara) => 3;
        protected override bool CanDoCore(Character chara) => true;

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            var targetArea = default(Area);
            var targetCountry = default(Country);

            if (chara.IsPlayer)
            {
                targetArea = await UI.ShowSelectAreaScreen(
                    World.Map.Areas,
                    World,
                    "奪取する地域を選択してください。");
                if (targetArea == null)
                {
                    return;
                }
                targetCountry = World.CountryOf(targetArea);
            }
            else
            {
                // ランダムに選ぶ。
                targetArea = World.Map.Areas.RandomPick();
                targetCountry = World.CountryOf(targetArea);
            }

            // ランダムな隣接エリアから攻撃する。
            var sourceArea = World.Map.GetNeighbors(targetArea).RandomPick();

            var sourcePosition = sourceArea.Position;
            var targetDirection = sourcePosition.GetDirectionTo(targetArea.Position);
            Core.Tilemap.ShowAttackDirectionArrow(sourceArea.Position, targetDirection);
            using var _ = Util.Defer(() => Core.Tilemap.HideAttackDirectionArrow());

            var attacker = chara;
            var newCountry = new Country
            {
                Id = World.Countries.Max(c => c.Id) + 1,
                ColorIndex = Enumerable.Range(0, 50)
                    .Except(World.Countries.Select(c => c.ColorIndex))
                    .RandomPick(),
                Areas = new List<Area>(),
                Ruler = chara,
                Vassals = new List<Character>(),
            };
            World.Countries.Add(newCountry);

            // 防衛側キャラを選択する。
            var defender = await Core.MartialActions.Attack.SelectDefender(
                attacker,
                sourceArea,
                newCountry,
                targetArea,
                targetCountry);

            // 侵攻する。
            var battle = BattleManager.Prepare(sourceArea, targetArea, attacker, defender);
            var result = await battle.Do();
            MartialActions.AttackAction.OnAfterAttack(battle, result, World);

            //// 攻撃済みフラグを立てる。
            //attacker.IsAttacked = true;

            // 負けたら国を削除する。
            if (result == BattleResult.DefenderWin)
            {
                World.Countries.Remove(newCountry);
            }

            if (targetCountry.Ruler.IsPlayer)
            {
                UI.MartialPhase.SetData(chara, World);
                UI.ShowMartialUI();
                Util.Todo();
            }
        }
    }
}
