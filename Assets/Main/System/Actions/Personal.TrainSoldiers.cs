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
    /// 兵士を訓練します。
    /// </summary>
    public TrainSoldiersAction TrainSoldiers { get; } = new();
    public class TrainSoldiersAction : PersonalActionBase
    {
        public override string Description => "兵士を訓練します。";

        public override int Cost(Character chara)
        {
            var averageLevel = chara.Force.Soldiers.Average(s => s.Level);
            return Mathf.Max(1, (int)averageLevel);
        }

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            foreach (var soldier in chara.Force.Soldiers)
            {
                if (soldier.IsEmptySlot) continue;
                soldier.AddExperience(chara);
            }

            PayCost(chara);
        }
    }
}
