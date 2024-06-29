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
    public OrganizeAction Organize { get; } = new();
    public class OrganizeAction : StrategyActionBase
    {
        public override string Description => "勢力の序列と給料配分を調整します。";

        public override int Cost(Character chara) => 1;
        protected override bool CanDoCore(Character chara) => true;

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));

            var country = World.CountryOf(chara);

            if (chara.IsPlayer)
            {
                await UI.ShowOrganizeScreen(country, World);
            }
            else
            {
                // 貢献順に並び替える。
                country.Vassals = country.Vassals.OrderByDescending(c => c.Contribution).ToList();

                // 給料配分を調整する。
                var salaryTable = table[country.Vassals.Count];
                for (int i = 0; i < country.Vassals.Count; i++)
                {
                    var vassal = country.Vassals[i];
                    vassal.SalaryRatio = salaryTable[i + 1];
                }
                country.RecalculateSalary();
            }

            PayCost(chara);
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
