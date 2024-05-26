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

    private async void MartialPhaseScreen_ActionButtonClicked(object sender, MartialPhaseScreen.ActionButton e)
    {
        var chara = MainUI.MartialPhase.debugCurrentChara;
        void OnActionEnd()
        {
            MainUI.MartialPhase.SetData(chara, world);
            MainUI.ShowMartialUI();
        }

        var mui = MainUI.MartialPhase;
        switch (e)
        {
            case MartialPhaseScreen.ActionButton.ShowInfo:
                MainUI.ShowCountryInfoScreen();
                break;
            case MartialPhaseScreen.ActionButton.Attack:
                if (MartialActions.Attack.CanDo(chara))
                {
                    await MartialActions.Attack.Do(chara);
                    OnActionEnd();
                }
                break;
            case MartialPhaseScreen.ActionButton.DecisiveBattle:
                //if (MartialActions.DecisiveBattle.CanDo(chara))
                //{
                //    MartialActions.DecisiveBattle.Do(chara);
                //    OnActionEnd();
                //}
                break;
            case MartialPhaseScreen.ActionButton.Provoke:
                if (MartialActions.Provoke.CanDo(chara))
                {
                    await MartialActions.Provoke.Do(chara);
                    OnActionEnd();
                }
                break;
            case MartialPhaseScreen.ActionButton.Subdue:
                if (MartialActions.Subdue.CanDo(chara))
                {
                    await MartialActions.Subdue.Do(chara);
                    OnActionEnd();
                }
                break;
            case MartialPhaseScreen.ActionButton.PrivateFight:
                if (MartialActions.PrivateFight.CanDo(chara))
                {
                    await MartialActions.PrivateFight.Do(chara);
                    OnActionEnd();
                }
                break;
            case MartialPhaseScreen.ActionButton.ShowSystemMenu:
                break;
            case MartialPhaseScreen.ActionButton.EndTurn:
                hold = false;
                break;
        }
    }

    private async void StrategyPhaseScreen_ActionButtonClicked(object sender, StrategyPhaseScreen.ActionButton e)
    {
        var chara = MainUI.StrategyPhase.debugCurrentChara;
        void OnActionEnd()
        {
            MainUI.StrategyPhase.SetData(chara, world);
            MainUI.ShowStrategyUI();
        }
        
        switch (e)
        {
            case StrategyPhaseScreen.ActionButton.ShowInfo:
                MainUI.ShowCountryInfoScreen();
                break;
            case StrategyPhaseScreen.ActionButton.Organize:
                if (StrategyActions.Organize.CanDo(chara))
                {
                    await StrategyActions.Organize.Do(chara);
                    OnActionEnd();
                }
                break;
            case StrategyPhaseScreen.ActionButton.HireVassal:
                if (StrategyActions.HireVassal.CanDo(chara))
                {
                    await StrategyActions.HireVassal.Do(chara);
                    OnActionEnd();
                }
                break;
            case StrategyPhaseScreen.ActionButton.FireVassal:
                if (StrategyActions.FireVassal.CanDo(chara))
                {
                    await StrategyActions.FireVassal.Do(chara);
                    OnActionEnd();
                }
                break;
            case StrategyPhaseScreen.ActionButton.Ally:
                if (StrategyActions.Ally.CanDo(chara))
                {
                    await StrategyActions.Ally.Do(chara);
                    OnActionEnd();
                }
                break;
            case StrategyPhaseScreen.ActionButton.ChangeAllianceStance:
                if (StrategyActions.ChangeAllianceStance.CanDo(chara))
                {
                    await StrategyActions.ChangeAllianceStance.Do(chara);
                    OnActionEnd();
                }
                break;
            case StrategyPhaseScreen.ActionButton.Resign:
                if (StrategyActions.Resign.CanDo(chara))
                {
                    await StrategyActions.Resign.Do(chara);
                    OnActionEnd();
                }
                break;
            case StrategyPhaseScreen.ActionButton.ShowSystemMenu:
                break;
            case StrategyPhaseScreen.ActionButton.EndTurn:
                hold = false;
                break;
        }
    }

    private async void IndividualPhaseScreen_ActionButtonClicked(object sender, IndividualPhaseScreen.ActionButton e)
    {
        var chara = MainUI.IndividualPhase.debugCurrentChara;
        void OnActionEnd()
        {
            MainUI.IndividualPhase.SetData(chara, world);
            MainUI.ShowIndividualUI();
        }

        switch (e)
        {
            case IndividualPhaseScreen.ActionButton.ShowInfo:
                MainUI.ShowCountryInfoScreen();
                break;
            case IndividualPhaseScreen.ActionButton.HireSoldier:
                if (PersonalActions.HireSoldier.CanDo(chara))
                {
                    await PersonalActions.HireSoldier.Do(chara);
                    OnActionEnd();
                }
                break;
            case IndividualPhaseScreen.ActionButton.TrainSoldiers:
                if (PersonalActions.TrainSoldiers.CanDo(chara))
                {
                    await PersonalActions.TrainSoldiers.Do(chara);
                    OnActionEnd();
                }
                break;
            case IndividualPhaseScreen.ActionButton.GetJob:
                if (PersonalActions.GetJob.CanDo(chara))
                {
                    await PersonalActions.GetJob.Do(chara);
                    OnActionEnd();
                }
                break;
            case IndividualPhaseScreen.ActionButton.Resign:
                if (PersonalActions.Resign.CanDo(chara))
                {
                    await PersonalActions.Resign.Do(chara);
                    OnActionEnd();
                }
                break;
            case IndividualPhaseScreen.ActionButton.Rebel:
                if (PersonalActions.Rebel.CanDo(chara))
                {
                    await PersonalActions.Rebel.Do(chara);
                    OnActionEnd();
                }
                break;
            case IndividualPhaseScreen.ActionButton.BecomeIndependent:
                if (PersonalActions.BecomeIndependent.CanDo(chara))
                {
                    await PersonalActions.BecomeIndependent.Do(chara);
                    OnActionEnd();
                }
                break;
            case IndividualPhaseScreen.ActionButton.ShowSystemMenu:
                break;
            case IndividualPhaseScreen.ActionButton.EndTurn:
                hold = false;
                break;
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
