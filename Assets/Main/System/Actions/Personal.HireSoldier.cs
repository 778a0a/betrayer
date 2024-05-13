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
    /// 兵士を雇います。
    /// </summary>
    public HireSoldierAction HireSoldier { get; } = new();
    public class HireSoldierAction : PersonalActionBase
    {
        public override int Cost(Character chara) => 5;
        protected override bool CanDoCore(Character chara) => chara.Force.HasEmptySlot;

        public override async Awaitable Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            var targetSlot = chara.Force.Soldiers.First(s => s.IsEmptySlot);
            targetSlot.IsEmptySlot = false;
            targetSlot.Level = 1;
            targetSlot.Experience = 0;
            targetSlot.Hp = targetSlot.MaxHp;
        }
    }
}
