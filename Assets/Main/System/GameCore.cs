using System;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using System.Linq;

public class GameCore
{
    /// <summary>
    /// プロトタイピング・緊急ハッチ用。基本的には使わない。
    /// </summary>
    public static GameCore Instance { get; set; }

    // ゲームクリアしたことがあるならtrue
    public static bool GameCleared
    {
        set => PlayerPrefs.SetInt("GameCleared", value ? 1 : 0);
        get => PlayerPrefs.GetInt("GameCleared", 0) == 1;
    }


    public WorldData World { get; private set; }
    public MainUI MainUI { get; private set; }
    public TilemapManager Tilemap { get; private set; }
    private Test test;

    public PhaseManager Phases { get; private set; }
    public StrategyActions StrategyActions { get; private set; }
    public PersonalActions PersonalActions { get; private set; }
    public MartialActions MartialActions { get; private set; }
    public CommonActions CommonActions { get; private set; }

    public int TurnCount { get; private set; } = 1;
    public PhaseBase CurrentPhase { get; private set; }
    public int SaveDataSlotNo { get; set; }

    private bool IsResumingGame { get; set; } = false;
    private SavedGameCoreState ResumingGameState { get; set; } = null;
    public bool IsFirstTurnAfterResume { get; set; }

    public GameCore(
        WorldData world,
        MainUI mainUI,
        TilemapManager tilemap,
        Test test,
        SavedGameCoreState state,
        int saveDataSlotNo)
    {
        World = world;
        MainUI = mainUI;
        Tilemap = tilemap;
        SaveDataSlotNo = saveDataSlotNo;
        this.test = test;
        if (state != null)
        {
            IsResumingGame = true;
            ResumingGameState = state;
            TurnCount = state.TurnCount;
            IsFirstTurnAfterResume = true;
        }

        Phases = new(this, test);
        PersonalActions = new(this);
        StrategyActions = new(this);
        MartialActions = new(this);
        CommonActions = new(this);
    }

    private async ValueTask DoPhase(PhaseBase phase)
    {
        if (IsResumingGame)
        {
            if (!ResumingGameState.IsTargetPhase(phase))
            {
                Debug.Log("[再開中]実行済みのフェイズをスキップします。" + phase.GetType().Name);
                return;
            }
            var order = ResumingGameState.RestoreActionOrder(World.Characters);
            phase.SetCustomActionOrder(order);

            IsResumingGame = false;
            ResumingGameState = null;
            foreach (var c in World.Countries)
            {
                Tilemap.SetExhausted(c, c.IsExhausted);
            }
        }
        else
        {
            phase.SetActionOrder();
        }

        CurrentPhase = phase;
        await test.HoldIfNeeded();
        await phase.Phase();
        CurrentPhase = null;
    }

    public async ValueTask DoMainLoop()
    {
        try
        {
            while (true)
            {
                await DoPhase(Phases.Start);
                await DoPhase(Phases.Income);
                await DoPhase(Phases.StrategyAction);
                await DoPhase(Phases.PersonalAction);
                await DoPhase(Phases.MartialAction);

                Tilemap.DrawCountryTile();
                if (World.Countries.Count == 1)
                {
                    var winner = World.Countries[0];
                    Debug.Log($"ゲーム終了 勝者: {winner} ターン数: {TurnCount}");
                    foreach (var c in winner.Members)
                    {
                        if (c.IsPlayer)
                        {
                            GameCleared = true;
                            break;
                        }
                    }
                    return;
                }

                await Awaitable.WaitForSecondsAsync(test.wait);

                if (test.holdOnTurnEnd)
                {
                    await test.WaitUserInteraction();
                }

                TurnCount++;
                IsFirstTurnAfterResume = false;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("メインループでエラー");
            Debug.LogException(ex);
        }
    }
}
