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
    /// 勢力を捨てて自由になります。
    /// </summary>
    public ResignAction Resign { get; } = new();
    public class ResignAction : PersonalActionBase
    {
        public override string Description => L["勢力を捨てて自由になります。"];

        public override bool CanSelect(Character chara) => World.IsVassal(chara); // Rulerは戦略フェイズで可能
        public override int Cost(Character chara) => 1;
        protected override bool CanDoCore(Character chara) =>
            World.CountryOf(chara).Vassals.Count > 0;

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));

            if (chara.IsPlayer)
            {
                var res = await MessageWindow.Show(L["勢力を捨てて浪士になります。\nよろしいですか？"], MessageBoxButton.OkCancel);
                if (res != MessageBoxResult.Ok) return;
            }
            var country = World.CountryOf(chara);
            country.Vassals.Remove(chara);
            country.RecalculateSalary();
            country.Ruler.AddUrami(10);

            Core.Tilemap.DrawCountryTile();
            PayCost(chara);
        }
    }

}
