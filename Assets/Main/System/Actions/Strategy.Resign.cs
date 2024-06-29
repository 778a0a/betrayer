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
    /// 勢力を捨てて放浪します。
    /// </summary>
    public ResignAction Resign { get; } = new();
    public class ResignAction : StrategyActionBase
    {
        public override string Description => "勢力を捨てて放浪します。";

        public override int Cost(Character chara) => 1;
        protected override bool CanDoCore(Character chara)
        {
            var country = World.CountryOf(chara);
            return country.Vassals.Count > 0 &&
                country.Areas.Count > 1;
        }

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));

            if (chara.IsPlayer)
            {
                var res = await MessageWindow.Show("勢力を捨てて放浪します。\nよろしいですか？", MessageBoxButton.OkCancel);
                if (res != MessageBoxResult.Ok) return;
            }

            var country = World.CountryOf(chara);
            var successor = country.Vassals[0];

            country.Vassals.RemoveAt(0);
            country.Ruler = successor;
            country.RecalculateSalary();
            await MessageWindow.Show($"{chara.Name}が勢力を捨て、\n{successor.Name}が新たな君主となりました。");

            Core.Tilemap.DrawCountryTile();

            PayCost(chara);
        }
    }
}
