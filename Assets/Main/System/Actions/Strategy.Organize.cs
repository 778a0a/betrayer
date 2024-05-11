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
    /// 勢力の序列と給料配分を調整します。
    /// </summary>
    public static OrganizeAction Organize { get; } = new();
    public class OrganizeAction : StrategyActionBase
    {
        public override int Cost(Character chara) => 0;
        protected override bool CanDoCore(Character chara) => true;

        public override async Awaitable Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            var country = World.CountryOf(chara);

            if (chara.IsPlayer)
            {
                await Test.Instance.MainUI.ShowOrganizeScreen(country, World);
                Test.Instance.MainUI.ShowStrategyUI();
            }
            else
            {
                // 貢献順に並び替える。
                country.Vassals = country.Vassals.OrderBy(c => c.Contribution).ToList();

                // 給料配分を調整する。
                var salaryTable = table[country.Vassals.Count];
                for (int i = 0; i < country.Vassals.Count; i++)
                {
                    var vassal = country.Vassals[i];
                    vassal.SalaryRatio = salaryTable[i + 1];
                }
                country.RecalculateSalary();
            }
        }

        private static readonly int[][] table = new[]
        {
            new[] {100, },
            new[] {70, 30, },
            new[] {55, 25, 20, },
            new[] {47, 20, 18, 15, },
            new[] {40, 18, 16, 14, 12, },
            new[] {39, 16, 14, 12, 10, 9, },
            new[] {35, 14, 13, 11, 10, 9, 8, },
            new[] {30, 13, 12, 11, 10, 9, 8, 7, },
            new[] {29, 12, 11, 10, 9, 8, 7, 7, 7, },
            new[] {22, 12, 11, 10, 9, 8, 7, 7, 7, 7, },
        };
    }
}
