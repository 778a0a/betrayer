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
        public override bool CanSelect(Character chara) => World.IsRuler(chara);
        public override int Cost(Character chara) => 5;
        protected override bool CanDoCore(Character chara) =>
            !chara.IsAttacked &&
            World.CountryOf(chara).Vassals.Count > 0;

        public override async Awaitable Do(Character chara)
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
                var result = await BattleManager.Battle(World.Map, attack, defend, chara, target);
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
                var result = await BattleManager.Battle(World.Map, attack, defend, chara, target);
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
}

