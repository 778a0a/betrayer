using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{
    public static Test Instance { get; private set; }

    [SerializeField] public Grid grid;
    [SerializeField] public Tilemap tilemap;

    [SerializeField] public TilesHolder tilesHolder;

    [SerializeField] public TilemapData initialTilemapData;

    [SerializeField] public WorldData world;

    [SerializeField] public float wait = 1;

    [SerializeField] public TilemapHelper tilemapHelper;

    [SerializeField] public MainUI MainUI;

    public PhaseManager phases;
    
    private object prevUI = null;

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

        world = SaveData.LoadWorldData(tilemapHelper);
        PersonalActions.Initialize(world);
        StrategyActions.Initialize(world);
        MartialActions.Initialize(world);
        phases = new PhaseManager(world);
        DrawCountryTile();

        MainUI.MainUIButtonClick += MainUI_MainUIButtonClick;
        MainUI.StrategyPhase.ActionButtonClicked += StrategyPhaseScreen_ActionButtonClicked;
        MainUI.IndividualPhase.ActionButtonClicked += IndividualPhaseScreen_ActionButtonClicked;
        MainUI.MartialPhase.ActionButtonClicked += MartialPhaseScreen_ActionButtonClicked;
        MainUI.CountryInfo.CloseButtonClicked += (sender, e) =>
        {
            if (prevUI == MainUI.IndividualPhase)
            { 
                MainUI.ShowIndividualUI();
            }
            else if (prevUI == MainUI.StrategyPhase)
            {
                MainUI.ShowStrategyUI();
            }
            else if (prevUI == MainUI.MartialPhase)
            {
                MainUI.ShowMartialUI();
            }
        };

        hold = true;
        holdOnTurnEnd = true;
        setHoldOnHoldEnd = true;
        StartCoroutine(DoMainLoop());
    }

    private void MartialPhaseScreen_ActionButtonClicked(object sender, MartialPhaseScreen.ActionButton e)
    {
        var chara = MainUI.MartialPhase.debugCurrentChara;
        var mui = MainUI.MartialPhase;
        switch (e)
        {
            case MartialPhaseScreen.ActionButton.ShowInfo:
                MainUI.ShowCountryInfoScreen();
                prevUI = mui;
                break;
            case MartialPhaseScreen.ActionButton.Attack:
                if (MartialActions.Attack.CanDo(chara))
                {
                    MartialActions.Attack.Do(chara);
                    mui.SetData(chara, world);
                }
                break;
            case MartialPhaseScreen.ActionButton.DecisiveBattle:
                //if (MartialActions.DecisiveBattle.CanDo(chara))
                //{
                //    MartialActions.DecisiveBattle.Do(chara);
                //    mui.SetData(chara, world);
                //}
                break;
            case MartialPhaseScreen.ActionButton.Provoke:
                if (MartialActions.Provoke.CanDo(chara))
                {
                    MartialActions.Provoke.Do(chara);
                    mui.SetData(chara, world);
                }
                break;
            case MartialPhaseScreen.ActionButton.Subdue:
                if (MartialActions.Subdue.CanDo(chara))
                {
                    MartialActions.Subdue.Do(chara);
                    mui.SetData(chara, world);
                }
                break;
            case MartialPhaseScreen.ActionButton.PrivateFight:
                if (MartialActions.PrivateFight.CanDo(chara))
                {
                    MartialActions.PrivateFight.Do(chara);
                    mui.SetData(chara, world);
                }
                break;
            case MartialPhaseScreen.ActionButton.ShowSystemMenu:
                break;
            case MartialPhaseScreen.ActionButton.EndTurn:
                hold = false;
                break;
        }
    }

    private void StrategyPhaseScreen_ActionButtonClicked(object sender, StrategyPhaseScreen.ActionButton e)
    {
        var chara = MainUI.StrategyPhase.debugCurrentChara;
        var straUI = MainUI.StrategyPhase;
        switch (e)
        {
            case StrategyPhaseScreen.ActionButton.ShowInfo:
                MainUI.ShowCountryInfoScreen();
                prevUI = straUI;
                break;
            case StrategyPhaseScreen.ActionButton.Organize:
                if (StrategyActions.Organize.CanDo(chara))
                {
                    StrategyActions.Organize.Do(chara);
                    straUI.SetData(chara, world);
                }
                break;
            case StrategyPhaseScreen.ActionButton.HireVassal:
                if (StrategyActions.HireVassal.CanDo(chara))
                {
                    StrategyActions.HireVassal.Do(chara);
                    straUI.SetData(chara, world);
                }
                break;
            case StrategyPhaseScreen.ActionButton.FireVassal:
                if (StrategyActions.FireVassal.CanDo(chara))
                {
                    StrategyActions.FireVassal.Do(chara);
                    straUI.SetData(chara, world);
                }
                break;
            case StrategyPhaseScreen.ActionButton.Ally:
                if (StrategyActions.Ally.CanDo(chara))
                {
                    StrategyActions.Ally.Do(chara);
                    straUI.SetData(chara, world);
                }
                break;
            case StrategyPhaseScreen.ActionButton.Resign:
                if (StrategyActions.Resign.CanDo(chara))
                {
                    StrategyActions.Resign.Do(chara);
                    straUI.SetData(chara, world);
                }
                break;
            case StrategyPhaseScreen.ActionButton.ShowSystemMenu:
                break;
            case StrategyPhaseScreen.ActionButton.EndTurn:
                hold = false;
                break;
        }
    }

    private void IndividualPhaseScreen_ActionButtonClicked(object sender, IndividualPhaseScreen.ActionButton e)
    {
        var chara = MainUI.IndividualPhase.debugCurrentChara;
        var indivUI = MainUI.IndividualPhase;
        switch (e)
        {
            case IndividualPhaseScreen.ActionButton.ShowInfo:
                MainUI.ShowCountryInfoScreen();
                prevUI = indivUI;
                break;
            case IndividualPhaseScreen.ActionButton.HireSoldier:
                if (PersonalActions.HireSoldier.CanDo(chara))
                {
                    PersonalActions.HireSoldier.Do(chara);
                    indivUI.SetData(chara, world);
                }
                break;
            case IndividualPhaseScreen.ActionButton.TrainSoldiers:
                if (PersonalActions.TrainSoldiers.CanDo(chara))
                {
                    PersonalActions.TrainSoldiers.Do(chara);
                    indivUI.SetData(chara, world);
                }
                break;
            case IndividualPhaseScreen.ActionButton.GetJob:
                if (PersonalActions.GetJob.CanDo(chara))
                {
                    PersonalActions.GetJob.Do(chara);
                    indivUI.SetData(chara, world);
                }
                break;
            case IndividualPhaseScreen.ActionButton.Resign:
                if (PersonalActions.Resign.CanDo(chara))
                {
                    PersonalActions.Resign.Do(chara);
                    indivUI.SetData(chara, world);
                }
                break;
            case IndividualPhaseScreen.ActionButton.Rebel:
                if (PersonalActions.Rebel.CanDo(chara))
                {
                    PersonalActions.Rebel.Do(chara);
                    indivUI.SetData(chara, world);
                }
                break;
            case IndividualPhaseScreen.ActionButton.BecomeIndependent:
                if (PersonalActions.BecomeIndependent.CanDo(chara))
                {
                    PersonalActions.BecomeIndependent.Do(chara);
                    indivUI.SetData(chara, world);
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

    private MapPosition prevPosition;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.GetRayIntersection(ray);
            if (hit.collider != null)
            {
                var posGrid = grid.WorldToCell(hit.point);
                var pos = MapPosition.FromGrid(posGrid);
                if (prevPosition != pos)
                {
                    prevPosition = pos;
                    MainUI.CountryInfo.ShowCellInformation(world, pos);
                }
            }
        }
    }

    private bool holdOnTurnEnd = false;
    private bool setHoldOnHoldEnd = false;
    public bool hold = false;
    public IEnumerator HoldIfNeeded()
    {
        while (hold)
        {
            yield return new WaitForSeconds(0.1f);
        }
        hold = setHoldOnHoldEnd;
    }


    // Update is called once per frame
    private IEnumerator DoMainLoop()
    {
        while (true)
        {
            yield return HoldIfNeeded();
            yield return phases.Start.Phase();
            
            yield return HoldIfNeeded();
            yield return phases.Income.Phase();

            yield return HoldIfNeeded();
            yield return phases.StrategyAction.Phase();

            yield return HoldIfNeeded();
            yield return phases.PersonalAction.Phase();

            yield return HoldIfNeeded();
            yield return phases.MartialAction.Phase();

            DrawCountryTile();
            if (world.Countries.Count == 1)
            {
                Debug.Log($"ゲーム終了 勝者: {world.Countries[0]}");
                yield break;
            }
            
            yield return new WaitForSeconds(wait);
            
            if (holdOnTurnEnd)
            {
                hold = true;
                yield return HoldIfNeeded();
            }
        }
    }

    private void DrawCountryTile()
    {
        var index2Tile = tilesHolder.countries;
        foreach (var country in world.Countries)
        {
            var colorIndex = country.ColorIndex;
            foreach (var area in country.Areas)
            {
                var pos = area.Position;
                tilemap.SetTile(pos.Vector3Int, index2Tile[country.ColorIndex]);
            }
        }
    }
}
