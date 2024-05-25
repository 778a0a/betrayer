﻿using System;
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
    /// 同盟を結びます。
    /// </summary>
    public AllyAction Ally { get; } = new();
    public class AllyAction : StrategyActionBase
    {
        public override int Cost(Character chara) => 5;
        protected override bool CanDoCore(Character chara)
        {
            var country = World.CountryOf(chara);
            return World.Countries.Count > 2 &&
                country.Ally == null &&
                World.Countries.Except(new[] { country }).Any(c => c.Ally == null);
        }

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara); // TODO

            var country = World.CountryOf(chara);

            if (chara.IsPlayer)
            {
                var ally = await UI.ShowSelectAllyScreen(country, World);
                if (ally == null)
                {
                    Debug.Log("同盟を結ぶ国が選択されませんでした。");

                }
                else
                {
                    country.Ally = ally;
                    ally.Ally = country;
                    Debug.Log($"{country} と {ally} が同盟を結びました。");
                }
                UI.ShowStrategyUI();
                return;
            }

            var neighbors = World.Neighbors(country);
            var target = default(Country);

            // 隣接国が複数ある場合は、その中からランダムに選ぶ。
            if (neighbors.Length > 1)
            {
                target = neighbors
                    .Where(c => c.Ally == null)
                    .RandomPickDefault();
            }
            // 隣接国が1つしかないか隣接国が同盟済みの場合は、他の国から選ぶ。
            if (target == null)
            {
                var cands = World.Countries
                    .Except(new[] { country })
                    .Except(neighbors)
                    .Where(c => c.Ally == null)
                    .ToList();
                target = cands.RandomPickDefault();
                if (target == null)
                {
                    Debug.Log($"{country} は同盟を結ぶ国がありませんでした。");
                    return;
                }
            }

            var ok = false;
            if (target.Ruler.IsPlayer)
            {
                Debug.Log($"{country} が {target} に同盟を申し込みました。");
                ok = await UI.ShowRespondAllyRequestScreen(country, World);
                UI.ShowStrategyUI();
            }
            else
            {
                ok = 0.5.Chance();
            }

            if (ok)
            {
                country.Ally = target;
                target.Ally = country;
                Debug.Log($"{country} と {target} が同盟を結びました。");
            }
            else
            {
                Debug.Log($"{country} が {target} に同盟を申し込みましたが、拒否されました。");
            }
        }
    }
}
