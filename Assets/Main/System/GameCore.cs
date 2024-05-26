using System;
using System.Collections;
using UnityEngine;
using static UnityEditor.ShaderData;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;

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

    public int TurnCount { get; private set; }

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

    public async ValueTask DoMainLoop(Test test)
    {
        try
        {
            TurnCount = 0;
            while (true)
            {
                await test.HoldIfNeeded();
                await Phases.Start.Phase();

                await test.HoldIfNeeded();
                await Phases.Income.Phase();

                await test.HoldIfNeeded();
                await Phases.StrategyAction.Phase();

                await test.HoldIfNeeded();
                await Phases.PersonalAction.Phase();

                await test.HoldIfNeeded();
                await Phases.MartialAction.Phase();

                Tilemap.DrawCountryTile();
                if (World.Countries.Count == 1)
                {
                    Debug.Log($"ゲーム終了 勝者: {World.Countries[0]} ターン数: {TurnCount}");
                    return;
                }

                await Awaitable.WaitForSecondsAsync(test.wait);

                if (test.holdOnTurnEnd)
                {
                    await test.WaitUserInteraction();
                }

                TurnCount++;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("メインループでエラー");
            Debug.LogException(ex);
        }
    }
}
