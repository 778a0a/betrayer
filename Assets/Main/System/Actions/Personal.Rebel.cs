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
    /// 反乱を起こします。
    /// </summary>
    public RebelAction Rebel { get; } = new();
    public class RebelAction : PersonalActionBase
    {
        public override string Description => L["反乱を起こします。"];

        public override bool CanSelect(Character chara) => World.IsVassal(chara);
        public override int Cost(Character chara) => 10;
        protected override bool CanDoCore(Character chara) => true;

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));

            if (chara.IsPlayer)
            {
                var res = await MessageWindow.Show(L["本当に反乱を起こしますか？"], MessageBoxButton.OkCancel);
                if (res != MessageBoxResult.Ok) return;
            }

            var country = World.CountryOf(chara);
            var ruler = country.Ruler;
            country.Vassals.Remove(chara);

            var newCountry = new Country()
            {
                Id = World.Countries.Max(c => c.Id) + 1,
                ColorIndex = Enumerable.Range(0, 50)
                    .Except(World.Countries.Select(c => c.ColorIndex))
                    .RandomPick(),
                Areas = new List<Area>(),
                Ruler = chara,
                Vassals = new List<Character>(),
            };
            World.Countries.Add(newCountry);

            // 反乱に加担するか確認する。
            var asked = false;
            foreach (var vassal in country.Vassals.ToList())
            {
                if (vassal.IsPlayer)
                {
                    var res = await MessageWindow.Show($"{chara.Name}が君主に対して反乱を起こしました！\n加担しますか？", MessageBoxButton.YesNo);
                    asked = true;
                    if (res == MessageBoxResult.Yes)
                    {
                        newCountry.Vassals.Add(vassal);
                        country.Vassals.Remove(vassal);
                    }
                }
                else
                {
                    var loyalty = vassal.Loyalty;
                    var percent = (100f - loyalty) / 30;
                    if (ruler.IsPlayer && vassal.Urami > 0) percent *= vassal.Urami;
                    if (percent.Chance())
                    {
                        newCountry.Vassals.Add(vassal);
                        country.Vassals.Remove(vassal);
                    }
                }
            }
            country.RecalculateSalary();
            newCountry.RecalculateSalary();

            if (!asked)
            {
                await MessageWindow.Show(L["{0}が{1}に対して反乱を起こしました！",
                    chara.Name,
                    ruler.Name]);
            }
            var kessen = Kessen.Prepare(newCountry, country);
            var result = await kessen.Do();

            // 勝ったら元の国を全て取得する。
            if (result == BattleResult.AttackerWin)
            {
                foreach (var area in country.Areas.ToList())
                {
                    newCountry.Areas.Add(area);
                    country.Areas.Remove(area);
                    Core.Tilemap.SetExhausted(area, newCountry.IsExhausted);
                }
                Core.Tilemap.DrawCountryTile();

                // 国を削除する。
                World.Countries.Remove(country);
                foreach (var c in World.Countries)
                {
                    if (c.Ally == country) c.Ally = null;
                }

                if (chara.IsPlayer)
                {
                    await MessageWindow.Show(L["反乱成功！\n新しい君主になりました。"]);
                    ruler.AddUrami(30);
                }
            }
            // 負けたら未所属になる。
            else
            {
                World.Countries.Remove(newCountry);
                if (chara.IsPlayer)
                {
                    await MessageWindow.Show(L["反乱は失敗し、勢力を追放されました。"]);
                    country.Ruler.AddUrami(30);
                }
            }

            Core.Tilemap.DrawCountryTile();
            PayCost(chara);
        }
    }
}
