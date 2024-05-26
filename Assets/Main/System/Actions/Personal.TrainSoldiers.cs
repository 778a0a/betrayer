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
        public override int Cost(Character chara)
        {
            var averageLevel = chara.Force.Soldiers.Average(s => s.Level);
            return (int)averageLevel;
        }

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            foreach (var soldier in chara.Force.Soldiers)
            {
                if (soldier.IsEmptySlot) continue;
                soldier.Experience += 1 + Random.Range(0, 0.3f);
                // 十分経験値が貯まればレベルアップする。
                if (soldier.Experience >= soldier.Level * 10 && soldier.Level < 13)
                {
                    soldier.Level += 1;
                    soldier.Experience = 0;
                    chara.Contribution += 1;
                }
            }

            PayCost(chara);
        }
    }
}
