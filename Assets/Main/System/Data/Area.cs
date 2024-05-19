﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 地域
/// </summary>
public class Area
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 地形
    /// </summary>
    public Terrain Terrain { get; set; }

    public MapPosition Position { get; set; }

    public Direction GetDirectionTo(Area target)
    {
        if (Position.x < target.Position.x) return Direction.Right;
        if (Position.x > target.Position.x) return Direction.Left;
        if (Position.y < target.Position.y) return Direction.Down;
        if (Position.y > target.Position.y) return Direction.Up;
        throw new InvalidOperationException();
    }

    public override string ToString() => $"Area {Position} ({Terrain})";
}

public struct MapPosition : IEquatable<MapPosition>
{
    public int x;
    public int y;

    public static MapPosition FromGrid(Vector3Int grid) => new() { x = grid.x, y = -grid.y };
    public static MapPosition Of(int x, int y) => new() { x = x, y = y };
    public readonly MapPosition Up => Of(x, y - 1);
    public readonly MapPosition Down => Of(x, y + 1);
    public readonly MapPosition Left => Of(x - 1, y);
    public readonly MapPosition Right => Of(x + 1, y);

    public readonly MapPosition To(Direction direction) => direction switch
    {
        Direction.Up => Up,
        Direction.Down => Down,
        Direction.Left => Left,
        Direction.Right => Right,
        _ => throw new ArgumentOutOfRangeException(nameof(direction)),
    };

    public readonly Direction GetDirectionTo(MapPosition pos)
    {
        if (pos.x < x) return Direction.Left;
        if (pos.x > x) return Direction.Right;
        if (pos.y < y) return Direction.Up;
        if (pos.y > y) return Direction.Down;
        throw new InvalidOperationException();
    }

    public readonly Vector3Int Vector3Int => new(x, -y, 0);

    public override readonly string ToString() => $"({x}, {y})";

    public readonly bool Equals(MapPosition other) => x == other.x && y == other.y;
    public static bool operator ==(MapPosition left, MapPosition right) => left.Equals(right);
    public static bool operator !=(MapPosition left, MapPosition right) => !(left == right);
    public override readonly bool Equals(object obj) => obj is MapPosition other && Equals(other);
    public override readonly int GetHashCode() => HashCode.Combine(x, y);

    public static readonly MapPosition Invalid = new() { x = -1, y = -1 };
    public readonly bool IsValid => this != Invalid;
}

/// <summary>
/// 地形
/// </summary>
public enum Terrain
{
    LargeRiver,
    River,
    Plain,
    Hill,
    Forest,
    Mountain,
    Fort,
}
