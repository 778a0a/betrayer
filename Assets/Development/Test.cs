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


        //StartCoroutine(DoMainLoop());
    }

    private MapPosition prevPosition;
    private void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hit = Physics2D.GetRayIntersection(ray);
        var posGrid = grid.WorldToCell(hit.point);
        var pos = MapPosition.FromGrid(posGrid);
        if (prevPosition != pos)
        {
            prevPosition = pos;
            rightPane.ShowCellInformation(world, pos);
        }
    }


    // Update is called once per frame
    private IEnumerator DoMainLoop()
    {
        while (true)
        {
            phases.Start.Phase();
            phases.Income.Phase();
            phases.PersonalAction.Phase();
            phases.StrategyAction.Phase();
            DrawCountryTile();
            if (world.Countries.Count == 1)
            {
                Debug.Log($"ゲーム終了 勝者: {world.Countries[0]}");
                yield break;
            }
            yield return new WaitForSeconds(wait);
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
