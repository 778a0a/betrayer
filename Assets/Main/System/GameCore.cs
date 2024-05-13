using System;
using System.Collections;
using UnityEngine;
using static UnityEditor.ShaderData;
using UnityEngine.Tilemaps;

public class GameCore
{
    /// <summary>
    /// プロトタイピング・緊急ハッチ用。基本的には使わない。
    /// </summary>
    public static GameCore Instance { get; set; }

    public WorldData World { get; private set; }
    public MainUI MainUI { get; private set; }
    public TilemapManager Tilemap { get; private set; }

    public PhaseManager Phases { get; private set; }
    public StrategyActions StrategyActions { get; private set; }
    public PersonalActions PersonalActions { get; private set; }
    public MartialActions MartialActions { get; private set; }

    public GameCore(MainUI mainUI, TilemapManager tilemap, Test test)
    {
        World = SaveData.LoadWorldData(tilemap.Helper);
        MainUI = mainUI;
        Tilemap = tilemap;

        Phases = new(this, test);
        PersonalActions = new(this);
        StrategyActions = new(this);
        MartialActions = new(this);
    }
}
