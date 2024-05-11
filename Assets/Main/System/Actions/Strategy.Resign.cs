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
    public static ResignAction Resign { get; } = new();
    public class ResignAction : StrategyActionBase
    {
        public override int Cost(Character chara) => 1;
        protected override bool CanDoCore(Character chara)
        {
            var country = World.CountryOf(chara);
            return country.Vassals.Count > 0;
        }

        public override async Awaitable Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            var country = World.CountryOf(chara);
            var successor = country.Vassals[0];

            country.Vassals.RemoveAt(0);
            country.Ruler = successor;
            country.RecalculateSalary();
            Debug.Log($"{chara.Name} が勢力を捨てて、{successor.Name} が新たな君主となりました。");
        }
    }
}
