using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Constraints;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{
    public static Test Instance { get; private set; }

    [SerializeField] private TilemapData initialTilemapData;

    [SerializeField] public float wait = 1;

    [SerializeField] private MainUI MainUI;

    [SerializeField] private TilemapManager tilemap;

    public bool showOthersBattle = true;

    private GameCore core;
    private WorldData world => core.World;
    private PhaseManager phases => core.Phases;
    private StrategyActions StrategyActions => core.StrategyActions;
    private PersonalActions PersonalActions => core.PersonalActions;
    private MartialActions MartialActions => core.MartialActions;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //world = DefaultData.InitializeDefaultData(initialTilemapData, tilemapHelper);
        //SaveData.SaveWorldData(world);
        //return;

        FaceImageManager.Instance.ClearCache();

        GameCore.Instance = core = new GameCore(MainUI, tilemap, this);
        tilemap.DrawCountryTile();

        MainUI.MainUIButtonClick += MainUI_MainUIButtonClick;
        MainUI.StrategyPhase.ActionButtonClicked += StrategyPhaseScreen_ActionButtonClicked;
        MainUI.IndividualPhase.ActionButtonClicked += IndividualPhaseScreen_ActionButtonClicked;
        MainUI.MartialPhase.ActionButtonClicked += MartialPhaseScreen_ActionButtonClicked;

        hold = false;
        holdOnTurnEnd = false;
        setHoldOnHoldEnd = false;
        
        core.DoMainLoop(this).Foreget();
    }

    private async void StrategyPhaseScreen_ActionButtonClicked(object sender, ActionButtonHelper button)
    {
        var chara = MainUI.StrategyPhase.debugCurrentChara;
        var action = button.Action;
        
        if (action is CommonActionBase)
        {
            if (action.CanDo(chara))
            {
                await action.Do(chara);
            }
            return;
        }
        
        if (action.CanDo(chara))
        {
            await action.Do(chara);
            OnActionEnd();
        }

        void OnActionEnd()
        {
            MainUI.StrategyPhase.SetData(chara, world);
            MainUI.ShowStrategyUI();
        }
    }

    private async void IndividualPhaseScreen_ActionButtonClicked(object sender, ActionButtonHelper button)
    {
        var chara = MainUI.IndividualPhase.debugCurrentChara;
        var action = button.Action;

        if (action is CommonActionBase)
        {
            if (action.CanDo(chara))
            {
                await action.Do(chara);
            }
            return;
        }

        if (action.CanDo(chara))
        {
            await action.Do(chara);
            OnActionEnd();
        }

        void OnActionEnd()
        {
            MainUI.IndividualPhase.SetData(chara, world);
            MainUI.ShowIndividualUI();
        }
    }

    private async void MartialPhaseScreen_ActionButtonClicked(object sender, ActionButtonHelper button)
    {
        var chara = MainUI.MartialPhase.debugCurrentChara;
        var action = button.Action;

        if (action is CommonActionBase)
        {
            if (action.CanDo(chara))
            {
                await action.Do(chara);
            }
            return;
        }

        if (action.CanDo(chara))
        {
            await action.Do(chara);
            OnActionEnd();
        }

        void OnActionEnd()
        {
            MainUI.MartialPhase.SetData(chara, world);
            MainUI.ShowMartialUI();
        }
    }

    private void MainUI_MainUIButtonClick(object sender, MainUI.MainUIButton e)
    {
        switch (e)
        {
            case MainUI.MainUIButton.ToggleDebugUI:
                MainUI.DebugUI.visible = !MainUI.DebugUI.visible;
                break;
            case MainUI.MainUIButton.NextPhase:
                hold = false;
                break;
            case MainUI.MainUIButton.NextTurn:
                hold = false;
                setHoldOnHoldEnd = false;
                holdOnTurnEnd = true;
                break;
            case MainUI.MainUIButton.Auto:
                hold = false;
                setHoldOnHoldEnd = false;
                holdOnTurnEnd = false;
                break;
            case MainUI.MainUIButton.Hold:
                hold = true;
                break;
        }
    }

    public void OnEnterStrategyPhase()
    {
        MainUI.ShowStrategyUI();
    }

    public void OnTickStrategyPhase(Character chara)
    {
        MainUI.StrategyPhase.SetData(chara, world);
    }

    public void OnEnterIndividualPhase()
    {
        MainUI.ShowIndividualUI();
    }

    public void OnTickIndividualPhase(Character chara)
    {
        MainUI.IndividualPhase.SetData(chara, world);
        //yield return new WaitForSeconds(0.05f);
    }

    public void OnEnterMartialPhase()
    {
        MainUI.ShowMartialUI();
    }

    public void OnTickMartialPhase(Character chara)
    {
        MainUI.MartialPhase.SetData(chara, world);
    }

    public bool holdOnTurnEnd = false;
    private bool setHoldOnHoldEnd = false;
    public bool hold = false;
    public async ValueTask HoldIfNeeded()
    {
        while (hold)
        {
            await Awaitable.WaitForSecondsAsync(0.1f);
        }
        hold = setHoldOnHoldEnd;
    }

    public async ValueTask WaitUserInteraction()
    {
        hold = true;
        await HoldIfNeeded();
    }
}
