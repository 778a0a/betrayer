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
        public override string Description => "自勢力から独立します。";

        public override bool CanSelect(Character chara) =>
            World.IsVassal(chara) &&
            World.CountryOf(chara).Areas.Count > 10 &&
            World.CountryOf(chara).Vassals.IndexOf(chara) == 0;
        public override int Cost(Character chara) => 5;
        protected override bool CanDoCore(Character chara) => true;

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));

            if (chara.IsPlayer)
            {
                var res = await MessageWindow.Show("本当に独立しますか？", MessageBoxButton.OkCancel);
                if (res != MessageBoxResult.Ok) return;
            }

            var oldCountry = World.CountryOf(chara);

            var areas = new List<Area>();
            var firstArea = oldCountry.Areas.RandomPick();
            areas.Add(firstArea);
            oldCountry.Areas.Remove(firstArea);

            var chance = chara.Intelligence / 100f;
            while (chance.Chance() && oldCountry.Areas.Count > 1)
            {
                chance *= 0.8f;

                var cand = areas.SelectMany(World.Map.GetNeighbors)
                    .Where(a => World.CountryOf(a) == oldCountry)
                    .Where(a => !areas.Contains(a))
                    .RandomPickDefault();
                if (cand != null)
                {
                    areas.Add(cand);
                    oldCountry.Areas.Remove(cand);
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

            Core.Tilemap.DrawCountryTile();
            PayCost(chara);

            if (chara.IsPlayer)
            {
                await MessageWindow.Show("独立しました。");
                oldCountry.Ruler.AddUrami(30);
            }
            else
            {
                await MessageWindow.Show($"{chara.Name}が{oldCountry.Ruler.Name}から独立しました。");
            }
        }
    }
}
