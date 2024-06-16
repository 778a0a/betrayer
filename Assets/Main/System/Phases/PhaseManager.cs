using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class PhaseManager
{
    public readonly StartPhase Start = new();
    public readonly IncomePhase Income = new();
    public readonly StrategyActionPhase StrategyAction = new();
    public readonly PersonalActionPhase PersonalAction = new();
    public readonly MartialActionPhase MartialAction = new();

    public PhaseBase[] Phases => new PhaseBase[] { Start, Income, StrategyAction, PersonalAction, MartialAction };
    public PhaseManager(GameCore core, Test test)
    {
        foreach (var phase in Phases)
        {
            phase.Core = core;
            phase.Test = test;
        }
    }
}

public class PhaseBase
{
    public GameCore Core { get; set; }
    public WorldData World => Core.World;
    public Character[] Characters => World.Characters;
    public IList<Country> Countries => World.Countries;
    public MapGrid Map => World.Map;
    public bool IsRuler(Character chara) => World.IsRuler(chara);
    public bool IsVassal(Character chara) => World.IsVassal(chara);
    public bool IsFree(Character chara) => World.IsFree(chara);

    public Test Test { get; set; }
    public StrategyActions StrategyActions => Core.StrategyActions;
    public PersonalActions PersonalActions => Core.PersonalActions;
    public MartialActions MartialActions => Core.MartialActions;

    public virtual ValueTask Phase() => new();

    public Character[] ActionOrder { get; protected set; }
    public virtual void SetActionOrder() { }
    public void SetCustomActionOrder(Character[] order) => ActionOrder = order;
}
