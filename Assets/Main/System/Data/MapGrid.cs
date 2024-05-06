using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// マップ
/// </summary>
public class MapGrid
{
    public TilemapHelper Helper { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Area[] Areas { get; set; }

    public Area GetIndexOf(int index) => Areas[index];
    public Area GetArea(MapPosition p) => GetIndexOf(GetIndex(p));
    public int GetIndex(MapPosition p) => p.y * Width + p.x;
    public MapPosition GetXY(int index) => MapPosition.Of(index % Width, index / Width);

    public bool IsValid(MapPosition p) => p.x >= 0 && p.x < Width && p.y >= 0 && p.y < Height;

    public Area GetAbove(Area area) => IsValid(area.Position.Up) ? GetArea(area.Position.Up) : null;
    public Area GetBelow(Area area) => IsValid(area.Position.Down) ? GetArea(area.Position.Down) : null;
    public Area GetLeft(Area area) => IsValid(area.Position.Left) ? GetArea(area.Position.Left) : null;
    public Area GetRight(Area area) => IsValid(area.Position.Right) ? GetArea(area.Position.Right) : null;

    public IEnumerable<Area> GetNeighbors(Area area)
    {
        if (IsValid(area.Position.Up)) yield return GetAbove(area);
        if (IsValid(area.Position.Down)) yield return GetBelow(area);
        if (IsValid(area.Position.Left)) yield return GetLeft(area);
        if (IsValid(area.Position.Right)) yield return GetRight(area);
    }
}
