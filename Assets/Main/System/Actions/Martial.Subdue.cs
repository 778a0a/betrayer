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
    /// 討伐
    /// </summary>
    public SubdueAction Subdue { get; } = new();
    public class SubdueAction : MartialActionBase
    {
        public override string Description => L["配下を討伐します。"];

        public override bool CanSelect(Character chara) => World.IsRuler(chara);
        public override int Cost(Character chara) => 5;
        protected override bool CanDoCore(Character chara) =>
            !chara.IsAttacked &&
            World.CountryOf(chara).Vassals.Count > 0;

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));

            var country = World.CountryOf(chara);

            // 戦う場所を決める。
            var (attack, defend) = country.Areas
                .SelectMany(a => World.Map.GetNeighbors(a)
                    .Where(country.Areas.Contains)
                    .Select(x => (a, x)))
                .DefaultIfEmpty((country.Areas[0], country.Areas[0]))
                .RandomPickDefault();

            var target = default(Character);
            if (chara.IsPlayer)
            {
                target = await UI.ShowSubdueScreen(country, World);
                if (target == null)
                {
                    Debug.Log("キャンセルされました。");
                    return;
                }
            }
            else
            {
                // 一番忠誠の低い配下を選ぶ。
                target = country.Vassals.OrderBy(c => c.Loyalty).First();
            }

            // 攻撃する。
            var battle = BattleManager.Prepare(attack, defend, chara, target, this);
            var result = await battle.Do();
            chara.IsAttacked = true;
            
            // 勝ったら配下を追放する。
            if (result == BattleResult.AttackerWin)
            {
                country.Vassals.Remove(target);
            }

            if (chara.IsPlayer)
            {
                await MessageWindow.Show(result == BattleResult.AttackerWin ?
                    L["討伐成功！\n{0}は追放されました。", target.Name] :
                    L["討伐失敗！{0}はあなたを恨んでいるようです。", target.Name]);
                target.AddUrami(30);
            }

            Core.Tilemap.DrawCountryTile();
            PayCost(chara);
        }
    }
}

