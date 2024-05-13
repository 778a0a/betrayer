using System.Collections;
using UnityEngine;

public class GameCore
{
    public WorldData World { get; set; }
    public PhaseManager Phases { get; set; }
    
    public StrategyActions StrategyActions { get; set; }
    public PersonalActions PersonalActions { get; set; }
    public MartialActions MartialActions { get; set; }

}
