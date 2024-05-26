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

            if (chara.IsPlayer)
            {
                // どのキャラを解雇するか選択する。
                var selected = await UI.ShowFireVassalUI(country, World);
                if (selected == null)
                {
                    Debug.Log("キャンセルされました。");
                    return;
                }
                country.Vassals.Remove(selected);
                country.RecalculateSalary();
            }
            else
            {
                // 一番弱い配下を解雇する。
                var target = country.Vassals.OrderBy(c => c.Power).First();
                country.Vassals.Remove(target);
                country.RecalculateSalary();
            }

            Core.Tilemap.DrawCountryTile();
            PayCost(chara);
        }
    }
}
