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
    public PhaseManager(Test test, WorldData world)
    {
        foreach (var phase in Phases)
        {
            phase.Test = test;
            phase.World = world;
        }
    }
}

public class PhaseBase
{
    public WorldData World { get; set; }
    public Character[] Characters => World.Characters;
    public List<Country> Countries => World.Countries;
    public MapGrid Map => World.Map;
    public bool IsRuler(Character chara) => World.IsRuler(chara);
    public bool IsVassal(Character chara) => World.IsVassal(chara);
    public bool IsFree(Character chara) => World.IsFree(chara);

    public Test Test { get; set; }

    public virtual async Awaitable Phase() { }
}
