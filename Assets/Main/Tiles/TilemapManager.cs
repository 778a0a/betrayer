using System;
using System.Linq;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class TilemapManager : MonoBehaviour
{
    private static MainUI MainUI => GameCore.Instance.MainUI;
    private static WorldData World => GameCore.Instance.World;

    [SerializeField] private Grid grid;
    [SerializeField] private TilesHolder tilesHolder;

    [SerializeField] private Tilemap countryTilemap;
    [SerializeField] private Tilemap terrainTilemap;
    [SerializeField] private Tilemap verticalConnectionTilemap;
    [SerializeField] private Tilemap horizontalConnectionTilemap;
    [SerializeField] private Tilemap uiTilemap;

    public TilemapHelper Helper { get; private set; }

    private void Awake()
    {
        Helper = new TilemapHelper(
            tilesHolder,
            terrainTilemap,
            verticalConnectionTilemap,
            horizontalConnectionTilemap,
            uiTilemap);
    }

    private MapPosition currentMousePosition = MapPosition.Of(0, 0);
    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hit = Physics2D.GetRayIntersection(ray);
        if (hit.collider != null)
        {
            var posGrid = grid.WorldToCell(hit.point);
            var pos = MapPosition.FromGrid(posGrid);

            if (currentMousePosition != pos)
            {
                var prevPos = currentMousePosition;
                currentMousePosition = pos;
                if (prevPos.IsValid) Helper.GetUITile(prevPos).SetCellBorder(false);
                Helper.GetUITile(pos).SetCellBorder(true);

            }

            if (Input.GetMouseButtonDown(0))
            {
                InvokeCellClickHandler(pos);
            }
        }
        else
        {
            if (currentMousePosition.IsValid)
            {
                Helper.GetUITile(currentMousePosition).SetCellBorder(false);
                currentMousePosition = MapPosition.Invalid;
            }
        }
    }

    private long currentClickHandlerId = 0;
    private EventHandler<MapPosition> cellClickHandler;
    private void DefaultCellClickHandler(object sender, MapPosition pos)
    {
        Debug.Log($"Clicked {pos}");
        MainUI.CountryInfo.ShowCellInformation(World, pos);
        MainUI.ShowCountryInfoScreen();
    }

    private void InvokeCellClickHandler(MapPosition pos)
    {
        if (cellClickHandler == null)
        {
            DefaultCellClickHandler(this, pos);
            return;
        }
        cellClickHandler?.Invoke(this, pos);
    }

    public IDisposable SetCellClickHandler(EventHandler<MapPosition> handler)
    {
        var id = currentClickHandlerId++;
        cellClickHandler = handler;
        return Util.Defer(() =>
        {
            if (currentClickHandlerId != id) return;
            cellClickHandler = null;
        });
    }


    public void DrawCountryTile()
    {
        var index2Tile = tilesHolder.countries;
        foreach (var country in World.Countries)
        {
            var colorIndex = country.ColorIndex;
            var isPlayer = country.IsPlayerCountry;
            foreach (var area in country.Areas)
            {
                var pos = area.Position;
                countryTilemap.SetTile(pos.Vector3Int, index2Tile[country.ColorIndex]);
                Helper.GetUITile(pos).SetPlayerCountry(isPlayer);
            }
        }
    }

    public void SetExhausted(Country country, bool exhausted)
    {
        foreach (var area in country.Areas)
        {
            var pos = area.Position;
            var tile = Helper.GetUITile(pos); 
            tile.SetExhausted(exhausted);
        }
    }
    public void ResetExhausted()
    {
        foreach (var country in World.Countries)
        {
            SetExhausted(country, false);
        }
    }

    private Country currentActiveCountry;
    public void SetActiveCountry(Country country)
    {
        if (currentActiveCountry == country) return;
        var prevCountry = currentActiveCountry;
        currentActiveCountry = country;

        if (prevCountry != null)
        {
            foreach (var area in prevCountry.Areas)
            {
                var pos = area.Position;
                var tile = Helper.GetUITile(pos);
                tile.SetActiveCountry(false);
            }
        }

        if (country != null)
        {
            // プレーヤー国の場合はすでにハイライトがあるのでスキップする。
            if (!country.IsPlayerCountry)
            {
                foreach (var area in country.Areas)
                {
                    var pos = area.Position;
                    var tile = Helper.GetUITile(pos);
                    tile.SetActiveCountry(true);
                }
            }
        }
    }

    public void ResetActiveCountry() => SetActiveCountry(null);

    public void SetDisableIcon(Func<Country, bool> value)
    {
        foreach (var country in World.Countries)
        {
            var canSelect = value(country);
            foreach (var area in country.Areas)
            {
                var pos = area.Position;
                var tile = Helper.GetUITile(pos);
                tile.SetDisableSelection(canSelect);
            }
        }
    }

    public void ResetDisableIcon()
    {
        SetDisableIcon(_ => false);
    }
}

public class TilemapHelper
{
    private readonly TilesHolder tilesHolder;
    private readonly Tilemap terrainTilemap;
    private readonly Tilemap verticalConnectionTilemap;
    private readonly Tilemap horizontalConnectionTilemap;
    private readonly Tilemap uiTilemap;
    private readonly UITile[] uiTiles;

    public TilemapHelper(
        TilesHolder tilesHolder,
        Tilemap terrainTilemap,
        Tilemap verticalConnectionTilemap,
        Tilemap horizontalConnectionTilemap,
        Tilemap uiTilemap)
    {
        this.tilesHolder = tilesHolder;
        this.terrainTilemap = terrainTilemap;
        this.verticalConnectionTilemap = verticalConnectionTilemap;
        this.horizontalConnectionTilemap = horizontalConnectionTilemap;
        this.uiTilemap = uiTilemap;
        uiTiles = uiTilemap.GetComponentsInChildren<UITile>()
            .OrderBy(t => -t.transform.position.y)
            .ThenBy(t => t.transform.position.x)
            .ToArray();
    }

    public UITile GetUITile(MapPosition pos)
    {
        return uiTiles[TilemapData.Width * pos.y + pos.x];
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