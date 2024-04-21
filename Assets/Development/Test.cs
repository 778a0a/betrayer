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
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap tilemap;

    [SerializeField] private TilesHolder tilesHolder;

    [SerializeField] private TilemapData initialTilemapData;

    [SerializeField] private WorldData world;

    [SerializeField] private float wait = 1;

    [SerializeField] private TilemapHelper tilemapHelper;

    [SerializeField] private RightPane rightPane;

    private PhaseManager phases;

    // Start is called before the first frame update
    void Start()
    {
        //world = DefaultData.InitializeDefaultData(initialTilemapData, tilemapHelper);
        //SaveData.SaveWorldData(world);
        //return;

        FaceImageManager.Instance.ClearCache();

        world = SaveData.LoadWorldData(tilemapHelper);
        phases = new PhaseManager(world);
        DrawCountryTile();

        rightPane.RightPaneButtonClick += RightPane_RightPaneButtonClick;

        hold = true;
        holdOnTurnEnd = true;
        setHoldOnHoldEnd = true;
        StartCoroutine(DoMainLoop());
    }

    private void RightPane_RightPaneButtonClick(object sender, RightPane.RightPaneButton e)
    {
        switch (e)
        {
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
                    rightPane.ShowCellInformation(world, pos);
                }
            }
        }
    }

    private bool holdOnTurnEnd = false;
    private bool setHoldOnHoldEnd = false;
    private bool hold = false;
    private IEnumerator HoldIfNeeded()
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
            phases.Start.Phase();
            
            yield return HoldIfNeeded();
            phases.Income.Phase();

            yield return HoldIfNeeded();
            phases.PersonalAction.Phase();
            
            yield return HoldIfNeeded();
            phases.StrategyAction.Phase();
            
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
