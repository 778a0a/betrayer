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

    [SerializeField] public RightPane rightPane;

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

        rightPane.RightPaneButtonClick += RightPane_RightPaneButtonClick;
        rightPane.StrategyPhaseUI.ActionButtonClicked += StrategyPhaseUI_ActionButtonClicked;
        rightPane.IndividualPhaseUI.ActionButtonClicked += IndividualPhaseUI_ActionButtonClicked;
        rightPane.MartialPhaseUI.ActionButtonClicked += MartialPhaseUI_ActionButtonClicked;
        rightPane.CountryInfo.CloseButtonClicked += (sender, e) =>
        {
            if (prevUI == rightPane.IndividualPhaseUI)
            { 
                rightPane.ShowIndividualUI();
            }
            else if (prevUI == rightPane.StrategyPhaseUI)
            {
                rightPane.ShowStrategyUI();
            }
            else if (prevUI == rightPane.MartialPhaseUI)
            {
                rightPane.ShowMartialUI();
            }
        };

        hold = true;
        holdOnTurnEnd = true;
        setHoldOnHoldEnd = true;
        StartCoroutine(DoMainLoop());
    }

    private void MartialPhaseUI_ActionButtonClicked(object sender, MartialPhaseUI.ActionButton e)
    {
        var chara = rightPane.MartialPhaseUI.debugCurrentChara;
        var mui = rightPane.MartialPhaseUI;
        switch (e)
        {
            case MartialPhaseUI.ActionButton.ShowInfo:
                rightPane.ShowCountryInfo();
                prevUI = mui;
                break;
            case MartialPhaseUI.ActionButton.Attack:
                if (MartialActions.Attack.CanDo(chara))
                {
                    MartialActions.Attack.Do(chara);
                    mui.SetData(chara, world);
                }
                break;
            case MartialPhaseUI.ActionButton.DecisiveBattle:
                //if (MartialActions.DecisiveBattle.CanDo(chara))
                //{
                //    MartialActions.DecisiveBattle.Do(chara);
                //    mui.SetData(chara, world);
                //}
                break;
            case MartialPhaseUI.ActionButton.Provoke:
                if (MartialActions.Provoke.CanDo(chara))
                {
                    MartialActions.Provoke.Do(chara);
                    mui.SetData(chara, world);
                }
                break;
            case MartialPhaseUI.ActionButton.Subdue:
                if (MartialActions.Subdue.CanDo(chara))
                {
                    MartialActions.Subdue.Do(chara);
                    mui.SetData(chara, world);
                }
                break;
            case MartialPhaseUI.ActionButton.PrivateFight:
                if (MartialActions.PrivateFight.CanDo(chara))
                {
                    MartialActions.PrivateFight.Do(chara);
                    mui.SetData(chara, world);
                }
                break;
            case MartialPhaseUI.ActionButton.ShowSystemMenu:
                break;
            case MartialPhaseUI.ActionButton.EndTurn:
                hold = false;
                break;
        }
    }

    private void StrategyPhaseUI_ActionButtonClicked(object sender, StrategyPhaseUI.ActionButton e)
    {
        var chara = rightPane.StrategyPhaseUI.debugCurrentChara;
        var straUI = rightPane.StrategyPhaseUI;
        switch (e)
        {
            case StrategyPhaseUI.ActionButton.ShowInfo:
                rightPane.ShowCountryInfo();
                prevUI = straUI;
                break;
            case StrategyPhaseUI.ActionButton.Organize:
                if (StrategyActions.Organize.CanDo(chara))
                {
                    StrategyActions.Organize.Do(chara);
                    straUI.SetData(chara, world);
                }
                break;
            case StrategyPhaseUI.ActionButton.HireVassal:
                if (StrategyActions.HireVassal.CanDo(chara))
                {
                    StrategyActions.HireVassal.Do(chara);
                    straUI.SetData(chara, world);
                }
                break;
            case StrategyPhaseUI.ActionButton.FireVassal:
                if (StrategyActions.FireVassal.CanDo(chara))
                {
                    StrategyActions.FireVassal.Do(chara);
                    straUI.SetData(chara, world);
                }
                break;
            case StrategyPhaseUI.ActionButton.Ally:
                if (StrategyActions.Ally.CanDo(chara))
                {
                    StrategyActions.Ally.Do(chara);
                    straUI.SetData(chara, world);
                }
                break;
            case StrategyPhaseUI.ActionButton.Resign:
                if (StrategyActions.Resign.CanDo(chara))
                {
                    StrategyActions.Resign.Do(chara);
                    straUI.SetData(chara, world);
                }
                break;
            case StrategyPhaseUI.ActionButton.ShowSystemMenu:
                break;
            case StrategyPhaseUI.ActionButton.EndTurn:
                hold = false;
                break;
        }
    }

    private void IndividualPhaseUI_ActionButtonClicked(object sender, IndividualPhaseUI.ActionButton e)
    {
        var chara = rightPane.IndividualPhaseUI.debugCurrentChara;
        var indivUI = rightPane.IndividualPhaseUI;
        switch (e)
        {
            case IndividualPhaseUI.ActionButton.ShowInfo:
                rightPane.ShowCountryInfo();
                prevUI = indivUI;
                break;
            case IndividualPhaseUI.ActionButton.HireSoldier:
                if (PersonalActions.HireSoldier.CanDo(chara))
                {
                    PersonalActions.HireSoldier.Do(chara);
                    indivUI.SetData(chara, world);
                }
                break;
            case IndividualPhaseUI.ActionButton.TrainSoldiers:
                if (PersonalActions.TrainSoldiers.CanDo(chara))
                {
                    PersonalActions.TrainSoldiers.Do(chara);
                    indivUI.SetData(chara, world);
                }
                break;
            case IndividualPhaseUI.ActionButton.GetJob:
                if (PersonalActions.GetJob.CanDo(chara))
                {
                    PersonalActions.GetJob.Do(chara);
                    indivUI.SetData(chara, world);
                }
                break;
            case IndividualPhaseUI.ActionButton.Resign:
                if (PersonalActions.Resign.CanDo(chara))
                {
                    PersonalActions.Resign.Do(chara);
                    indivUI.SetData(chara, world);
                }
                break;
            case IndividualPhaseUI.ActionButton.Rebel:
                if (PersonalActions.Rebel.CanDo(chara))
                {
                    PersonalActions.Rebel.Do(chara);
                    indivUI.SetData(chara, world);
                }
                break;
            case IndividualPhaseUI.ActionButton.BecomeIndependent:
                if (PersonalActions.BecomeIndependent.CanDo(chara))
                {
                    PersonalActions.BecomeIndependent.Do(chara);
                    indivUI.SetData(chara, world);
                }
                break;
            case IndividualPhaseUI.ActionButton.ShowSystemMenu:
                break;
            case IndividualPhaseUI.ActionButton.EndTurn:
                hold = false;
                break;
        }
    }

    private void RightPane_RightPaneButtonClick(object sender, RightPane.RightPaneButton e)
    {
        switch (e)
        {
            case RightPane.RightPaneButton.ToggleDebugUI:
                rightPane.DebugUI.visible = !rightPane.DebugUI.visible;
                break;
            case RightPane.RightPaneButton.NextPhase:
                hold = false;
                break;
            case RightPane.RightPaneButton.NextTurn:
                hold = false;
                setHoldOnHoldEnd = false;
                holdOnTurnEnd = true;
                break;
            case RightPane.RightPaneButton.Auto:
                hold = false;
                setHoldOnHoldEnd = false;
                holdOnTurnEnd = false;
                break;
            case RightPane.RightPaneButton.Hold:
                hold = true;
                break;
        }
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
                    rightPane.CountryInfo.ShowCellInformation(world, pos);
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
