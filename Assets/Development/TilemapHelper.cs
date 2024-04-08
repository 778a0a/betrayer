using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapHelper : MonoBehaviour
{
    [SerializeField] private TilesHolder tilesHolder;

    [SerializeField] private Tilemap terrainTilemap;
    [SerializeField] private Tilemap verticalConnectionTilemap;
    [SerializeField] private Tilemap horizontalConnectionTilemap;

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