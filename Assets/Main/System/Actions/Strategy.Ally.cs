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
    /// 同盟を結びます。
    /// </summary>
    public AllyAction Ally { get; } = new();
    public class AllyAction : StrategyActionBase
    {
        public override string Description => L["他勢力と同盟を結びます。"];

        public override bool CanSelect(Character chara) =>
            base.CanSelect(chara) &&
            World.CountryOf(chara).Ally == null;
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

            var country = World.CountryOf(chara);
            var target = default(Country);

            if (chara.IsPlayer)
            {
                target = await UI.ShowSelectAllyScreen(country, World);
                if (target == null)
                {
                    Debug.Log("キャンセルされました。");
                    return;
                }
            }
            else
            {
                // 隣接国が複数ある場合は、その中からランダムに選ぶ。
                var neighbors = World.Neighbors(country);
                if (neighbors.Length > 1)
                {
                    target = neighbors
                        .Where(c => c.Ally == null)
                        .RandomPickDefault();
                }
                // 隣接国が1つしかないか隣接国が同盟済みの場合は、他の国から選ぶ。
                if (target == null)
                {
                    // まず隣接国の隣接国から選ぶ。
                    var cands = neighbors
                        .SelectMany(World.Neighbors)
                        .Except(neighbors)
                        .Where(c => c != country)
                        .Where(c => c.Ally == null)
                        .ToArray();

                    // 隣接国の隣接国がいない場合は、他の国から選ぶ。
                    if (cands.Length == 0)
                    {
                        cands = World.Countries
                            .Except(neighbors)
                            .Where(c => c != country)
                            .Where(c => c.Ally == null)
                            .ToArray();
                    }

                    target = cands.RandomPickDefault();
                    if (target == null)
                    {
                        Debug.Log($"{country} は同盟を結ぶ国がありませんでした。");
                        return;
                    }
                }
            }

            var ok = false;
            if (target.Ruler.IsPlayer)
            {
                Debug.Log($"{country} が {target} に同盟を申し込みました。");
                ok = await UI.ShowRespondAllyRequestScreen(country, World);
                UI.ShowStrategyUI();
                Util.Todo();
            }
            else
            {
                ok = 0.5.Chance();
                if (chara.IsPlayer && target.Ruler.Urami > 0)
                {
                    ok = false;
                }
            }

            if (ok)
            {
                country.Ally = target;
                country.WantsToContinueAlliance = true;
                target.Ally = country;
                target.WantsToContinueAlliance = true;
                Debug.Log($"{country} と {target} が同盟を結びました。");
                if (chara.IsPlayer)
                {
                    await MessageWindow.Show(L["{0}と同盟を結びました。", target.Name]);
                }
                if (target.Ruler.IsPlayer)
                {
                    await MessageWindow.Show(L["{0}と同盟を結びました。", country.Name]);
                }
            }
            else
            {
                Debug.Log($"{country} が {target} に同盟を申し込みましたが、拒否されました。");
                if (chara.IsPlayer)
                {
                    await MessageWindow.Show(L["拒否されました。"]);
                }
            }

            PayCost(chara);
        }
    }
}
