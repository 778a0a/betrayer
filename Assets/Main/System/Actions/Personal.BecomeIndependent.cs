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
    /// 大勢力から独立します。
    /// </summary>
    public BecomeIndependentAction BecomeIndependent { get; } = new();
    public class BecomeIndependentAction : PersonalActionBase
    {
        public override bool CanSelect(Character chara) =>
            World.IsVassal(chara) &&
            World.CountryOf(chara).Areas.Count > 10 &&
            World.CountryOf(chara).Vassals.IndexOf(chara) == 0;
        public override int Cost(Character chara) => 5;
        protected override bool CanDoCore(Character chara) => true;

        public override async Awaitable Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            var oldCountry = World.CountryOf(chara);
            var areas = new List<Area>
            {
                oldCountry.Areas.RandomPick(),
            };
            while (0.4.Chance())
            {
                var neighbor = World.Map.GetNeighbors(areas.RandomPick())
                    .Where(a => World.CountryOf(a) == oldCountry && !areas.Contains(a))
                    .RandomPickDefault();
                if (neighbor != null)
                {
                    areas.Add(neighbor);
                    oldCountry.Areas.Remove(neighbor);
                }
            }
            var newCountry = new Country
            {
                Id = World.Countries.Max(c => c.Id) + 1,
                ColorIndex = Enumerable.Range(0, 50).Except(World.Countries.Select(c => c.ColorIndex)).RandomPick(),
                Areas = areas,
                Ruler = chara,
                Vassals = new List<Character>(),
            };
            oldCountry.Vassals.Remove(chara);
            World.Countries.Add(newCountry);
        }
    }
}
