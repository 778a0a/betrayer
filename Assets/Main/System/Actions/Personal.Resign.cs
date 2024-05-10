using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Assertions;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

partial class PersonalActions
{
    /// <summary>
    /// 勢力を捨てて自由になります。
    /// </summary>
    public static ResignAction Resign { get; } = new();
    public class ResignAction : PersonalActionBase
    {
        public override bool CanSelect(Character chara) => World.IsVassal(chara); // Rulerは戦略フェイズで可能
        public override int Cost(Character chara) => 1;
        protected override bool CanDoCore(Character chara) =>
            World.CountryOf(chara).Vassals.Count > 0;

        public override void Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            var country = World.CountryOf(chara);
            if (country.Ruler == chara)
            {
                var newRuler = country.Vassals[0];
                country.Ruler = newRuler;
                country.Vassals.Remove(newRuler);
                country.RecalculateSalary();
            }
            else
            {
                country.Vassals.Remove(chara);
                country.RecalculateSalary();
            }
        }
    }

}
