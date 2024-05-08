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
    public static void Initialize(WorldData world)
    {
        foreach (var action in All)
        {
            action.World = world;
        }
    }

    /// <summary>
    /// 兵士を訓練します。
    /// </summary>
    public static TrainSoldiersAction TrainSoldiers { get; } = new();
    public class TrainSoldiersAction : PersonalActionBase
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
                if (soldier.Experience >= soldier.Level * 10 && soldier.Level < 13)
                {
                    soldier.Level += 1;
                    soldier.Experience = 0;
                    //soldier.Hp = soldier.MaxHp;
                }
            }
        }
    }

    /// <summary>
    /// 兵士を雇います。
    /// </summary>
    public static HireSoldierAction HireSoldier { get; } = new();
    public class HireSoldierAction : PersonalActionBase
    {
        public override int Cost(Character chara) => 5;
        protected override bool CanDoCore(Character chara) => chara.Force.HasEmptySlot;

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

    /// <summary>
    /// 既存勢力に仕官します。
    /// </summary>
    public static GetJobAction GetJob { get; } = new();
    public class GetJobAction : PersonalActionBase
    {
        public override bool CanSelect(Character chara) => World.IsFree(chara);
        public override int Cost(Character chara) => 10;
        protected override bool CanDoCore(Character chara) =>
            World.Countries.Where(c => c.VassalCountMax > c.Vassals.Count).Any();

        public override async void Do(Character chara)
        {
            Assert.IsTrue(CanSelect(chara));
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara); // TODO

            if (chara.IsPlayer)
            {
                var country = await Test.Instance.MainUI.ShowGetJobScreen(World);
                if (country != null)
                {
                    country.AddVassal(chara);
                }
                Test.Instance.MainUI.ShowIndividualUI();
                Test.Instance.MainUI.IndividualPhase.SetData(chara, World);
            }
            else
            {
                var countries = World.Countries.Where(c => c.VassalCountMax > c.Vassals.Count);
                var country = countries.RandomPick();
                country.AddVassal(chara);
            }
        }
    }

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

    /// <summary>
    /// 反乱を起こします。
    /// </summary>
    public static RebelAction Rebel { get; } = new();
    public class RebelAction : PersonalActionBase
    {
        public override bool CanSelect(Character chara) => World.IsVassal(chara);
        public override int Cost(Character chara) => 10;
        protected override bool CanDoCore(Character chara) => true;

        public override void Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            var country = World.CountryOf(chara);
            var ruler = country.Ruler;

            var target = country.Areas.RandomPick();
            var source = World.Map.GetNeighbors(target).RandomPick();
            var result = BattleManager.Battle(World.Map, source, target, chara, ruler);
            if (result == BattleResult.AttackerWin)
            {
                var oldRuler = country.Ruler;
                country.Ruler = chara;
                country.Vassals.Remove(chara);
                country.RecalculateSalary();
            }
            else
            {
                country.Vassals.Remove(chara);
                country.RecalculateSalary();
            }
        }
    }

    /// <summary>
    /// 大勢力から独立します。
    /// </summary>
    public static BecomeIndependentAction BecomeIndependent { get; } = new();
    public class BecomeIndependentAction : PersonalActionBase
    {
        public override bool CanSelect(Character chara) =>
            World.IsVassal(chara) &&
            World.CountryOf(chara).Areas.Count > 10 &&
            World.CountryOf(chara).Vassals.IndexOf(chara) == 0;
        public override int Cost(Character chara) => 5;
        protected override bool CanDoCore(Character chara) => true;

        public override void Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            var oldCountry = World.CountryOf(chara);
            var areas = new List<Area>
            {
                oldCountry.Areas.RandomPick(),
            };
            while (0.4.Chance())
            {
                var neighbor = World.Map.GetNeighbors(areas.RandomPick())
                    .Where(a => World.CountryOf(a) == oldCountry && !areas.Contains(a))
                    .RandomPickDefault();
                if (neighbor != null)
                {
                    areas.Add(neighbor);
                    oldCountry.Areas.Remove(neighbor);
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
        }
    }

    public static PersonalActionBase[] All { get; } = new PersonalActionBase[]
    {
        TrainSoldiers,
        HireSoldier,
        GetJob,
        Resign,
        Rebel,
        BecomeIndependent,
    };
}

public class PersonalActionBase
{
    public WorldData World { get; set; }

    /// <summary>
    /// 選択肢として表示可能ならtrue
    /// </summary>
    public virtual bool CanSelect(Character chara) => true;
    /// <summary>
    /// アクションの実行に必要なGold
    /// </summary>
    public virtual int Cost(Character chara) => 0;
    /// <summary>
    /// アクションを実行可能ならtrue
    /// </summary>
    public bool CanDo(Character chara) => CanSelect(chara) && chara.Gold >= Cost(chara) && CanDoCore(chara);
    /// <summary>
    /// アクションを実行可能ならtrue（子クラスでのオーバーライド用）
    /// </summary>
    /// <returns></returns>
    protected virtual bool CanDoCore(Character chara) => true;
    /// <summary>
    /// アクションを実行します。
    /// </summary>
    public virtual void Do(Character chara) { }
}
