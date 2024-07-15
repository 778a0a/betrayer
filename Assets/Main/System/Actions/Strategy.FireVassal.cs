using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

partial class StrategyActions
{
    /// <summary>
    /// 配下を解雇します。
    /// </summary>
    public FireVassalAction FireVassal { get; } = new();
    public class FireVassalAction : StrategyActionBase
    {
        public override string Description => L["配下を解雇します。"];

        public override int Cost(Character chara) => 1;
        protected override bool CanDoCore(Character chara)
        {
            var country = World.CountryOf(chara);
            return country.Vassals.Count > 0;
        }

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));

            var country = World.CountryOf(chara);
            var target = default(Character);
            if (chara.IsPlayer)
            {
                // どのキャラを解雇するか選択する。
                target = await UI.ShowFireVassalUI(country, World);
                if (target == null)
                {
                    Debug.Log("キャンセルされました。");
                    return;
                }
            }
            else
            {
                // 一番弱い配下を解雇する。
                target = country.Vassals.OrderBy(c => c.Power).First();
            }

            if (!target.IsPlayer)
            {
                // 戦力に応じて一定確率で拒否される。
                var powerBalance = (float)target.Power / chara.Power;
                if (powerBalance.Chance() || (chara.IsPlayer && target.Urami > 0))
                {
                    Debug.Log($"{target.Name}は{chara.Name}の解雇を拒否しました。");
                    if (chara.IsPlayer)
                    {
                        await MessageWindow.Show(L["拒否されました。"]);
                        target.AddUrami(30);
                    }
                    PayCost(chara);
                    return;
                }
            }

            country.Vassals.Remove(target);
            country.RecalculateSalary();
            Debug.Log($"{target.Name}は{chara.Name}によって追放されました。");

            if (chara.IsPlayer)
            {
                await MessageWindow.Show(L["{0}を追放しました。", target.Name]);
                target.AddUrami(30);
            }
            if (target.IsPlayer)
            {
                await MessageWindow.Show(L["あなたは勢力から追放されました。"]);
            }

            Core.Tilemap.DrawCountryTile();
            PayCost(chara);
        }
    }
}
