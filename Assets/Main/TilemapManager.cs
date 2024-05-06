using System;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    public event EventHandler<MapPosition> TileClick;

    [SerializeField] private Grid grid;
    [SerializeField] private TilesHolder tilesHolder;

    [SerializeField] private Tilemap countryTilemap;
    [SerializeField] private Tilemap terrainTilemap;
    [SerializeField] private Tilemap verticalConnectionTilemap;
    [SerializeField] private Tilemap horizontalConnectionTilemap;

    public TilemapHelper Helper { get; private set; }

    private void Awake()
    {
        Helper = new TilemapHelper(tilesHolder, terrainTilemap, verticalConnectionTilemap, horizontalConnectionTilemap);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.GetRayIntersection(ray);
            if (hit.collider != null)
            {
                var posGrid = grid.WorldToCell(hit.point);
                var pos = MapPosition.FromGrid(posGrid);
                TileClick?.Invoke(this, pos);
            }
        }
    }

    public void DrawCountryTile(WorldData world)
    {
        var index2Tile = tilesHolder.countries;
        foreach (var country in world.Countries)
        {
            var colorIndex = country.ColorIndex;
            foreach (var area in country.Areas)
            {
                var pos = area.Position;
                countryTilemap.SetTile(pos.Vector3Int, index2Tile[country.ColorIndex]);
            }
        }
    }
}

public class TilemapHelper
{
    private readonly TilesHolder tilesHolder;
    private readonly Tilemap terrainTilemap;
    private readonly Tilemap verticalConnectionTilemap;
    private readonly Tilemap horizontalConnectionTilemap;
    public TilemapHelper(
        TilesHolder tilesHolder,
        Tilemap terrainTilemap,
        Tilemap verticalConnectionTilemap,
        Tilemap horizontalConnectionTilemap)
    {
        this.tilesHolder = tilesHolder;
        this.terrainTilemap = terrainTilemap;
        this.verticalConnectionTilemap = verticalConnectionTilemap;
        this.horizontalConnectionTilemap = horizontalConnectionTilemap;
    }

    public Sprite GetCountryImage(Country country)
    {
        var tile = tilesHolder.countries[country.ColorIndex];
        return tile.sprite;
    }

    public Terrain GetTerrain(MapPosition pos)
    {
        var tile = terrainTilemap.GetTile(pos.Vector3Int);
        var index = Array.IndexOf(tilesHolder.terrains, tile);
        return Util.EnumArray<Terrain>()[index];
    }

    public Connection GetConnection(MapPosition pos, Direction direction)
    {
        // 012は水平方向の接続タイル、345は垂直方向の接続タイルが入っている。
        var connections = Util.EnumArray<Connection>();

        var tilemapPos = pos;
        if (direction == Direction.Left) tilemapPos = pos.Left;
        if (direction == Direction.Up) tilemapPos = pos.Up;

        if (direction == Direction.Left || direction == Direction.Right)
        {
            var tile = horizontalConnectionTilemap.GetTile(tilemapPos.Vector3Int);
            var index = Array.IndexOf(tilesHolder.connections, tile);
            return connections[index];
        }
        else
        {
            var tile = verticalConnectionTilemap.GetTile(tilemapPos.Vector3Int);
            var index = Array.IndexOf(tilesHolder.connections, tile);
            return connections[index - connections.Length];
        }
    }

    public Terrain GetAttackerTerrain(MapPosition attackSourcePos, Direction direction)
    {
        var terrain = GetTerrain(attackSourcePos);
        var connection = GetConnection(attackSourcePos, direction);
        if (connection == Connection.LargeRiver)
        {
            return Terrain.LargeRiver;
        }
        else if (connection == Connection.River)
        {
            return Terrain.River;
        }
        return terrain;
    }
}

public enum Connection
{
    Normal,
    LargeRiver,
    River,
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}