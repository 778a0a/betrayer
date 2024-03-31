using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Assertions;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

public class PersonalActions
{
    public static TrainSoldiersPersonalAction TrainSoldiers { get; } = new();
    public static HireSoldierPersonalAction HireSoldier { get; } = new();

}

/// <summary>
/// 兵士を訓練します。
/// </summary>
public class TrainSoldiersPersonalAction : PersonalActionBase
{
    public override int Cost(Character chara)
    {
        var averageLevel = chara.Force.Soldiers.Average(s => s.Level);
        return (int)averageLevel;
    }

    public override void Do(Character chara)
    {
        Assert.IsTrue(CanDo(chara));
        chara.Gold -= Cost(chara);
        foreach (var soldier in chara.Force.Soldiers)
        {
            if (soldier.IsEmptySlot) continue;
            soldier.Experience += 1;
            // 十分経験値が貯まればレベルアップする。
            if (soldier.Experience >= soldier.Level * 10)
            {
                soldier.Level += 1;
                soldier.Experience = 0;
                soldier.Hp = soldier.MaxHp;
            }
        }
    }
}

/// <summary>
/// 兵士を雇います。
/// </summary>
public class HireSoldierPersonalAction : PersonalActionBase
{
    public override int Cost(Character chara) => 5;
    public override bool CanDo(Character chara) => chara.Force.HasEmptySlot && chara.Gold >= Cost(chara);

    public override void Do(Character chara)
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


public class PersonalActionBase
{
    public virtual int Cost(Character chara) => 0;
    public virtual bool CanDo(Character chara) => chara.Gold >= Cost(chara);
    public virtual void Do(Character chara) { }
}
