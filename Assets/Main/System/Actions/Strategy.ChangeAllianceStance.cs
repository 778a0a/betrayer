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
    /// 同盟継続方針を更新します。
    /// </summary>
    public ChangeAllianceStanceAction ChangeAllianceStance { get; } = new();
    public class ChangeAllianceStanceAction : StrategyActionBase
    {
        public override string Description => L["同盟継続方針を更新します。"];

        public static int MaxTurnCountToDisableAlliance = 5;

        public override bool CanSelect(Character chara) =>
            base.CanSelect(chara) &&
            World.CountryOf(chara).Ally != null;
        public override int Cost(Character chara) => 0;
        protected override bool CanDoCore(Character chara)
        {
            var country = World.CountryOf(chara);
            return country.Ally != null;
        }

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));

            var country = World.CountryOf(chara);

            if (chara.IsPlayer)
            {
                country.WantsToContinueAlliance = !country.WantsToContinueAlliance;
                if (!country.WantsToContinueAlliance)
                {
                    country.TurnCountToDisableAlliance = MaxTurnCountToDisableAlliance;
                }
                Debug.Log($"{country.Name} は同盟継続方針を変更しました。継続?: {country.WantsToContinueAlliance}");
            }
            else
            {
                static bool ShouldContinue(Country country, WorldData World)
                {
                    var ally = country.Ally;

                    // 唯一の隣接国なら継続しない。
                    var neighbors = World.Neighbors(country);
                    if (neighbors.Length == 1 && neighbors[0] == ally)
                    {
                        return false;
                    }

                    // 遠いなら継続しない。
                    if (!neighbors.Contains(ally))
                    {
                        var neighbors2 = neighbors.SelectMany(World.Neighbors);
                        if (!neighbors2.Contains(ally))
                        {
                            return false;
                        }
                    }

                    return true;
                }

                var prev = country.WantsToContinueAlliance;
                var next = ShouldContinue(country, World);
                country.WantsToContinueAlliance = next;
                if (prev != next)
                {
                    Debug.Log($"{country.Name} は同盟継続方針を変更しました。継続?: {country.WantsToContinueAlliance}");
                    if (!next)
                    {
                        country.TurnCountToDisableAlliance = MaxTurnCountToDisableAlliance;
                    }
                }
            }

            PayCost(chara);
        }
    }
}
