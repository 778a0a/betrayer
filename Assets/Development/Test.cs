using System.Collections;
using System.Collections.Generic;
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

    private PhaseManager phases;

    // Start is called before the first frame update
    void Start()
    {
        world = DefaultData.InitializeDefaultData(initialTilemapData);
        phases = new PhaseManager(world);
        DrawCountryTile();

        StartCoroutine(DoMainLoop());
    }

    // Update is called once per frame
    private IEnumerator DoMainLoop()
    {
        while (true)
        {
            phases.Income.Phase();
            phases.PersonalAction.Phase();
            phases.StrategyAction.Phase();
            phases.End.Phase();
            DrawCountryTile();
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
