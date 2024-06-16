using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SavedMapPosition
{
    public int X { get; set; }
    public int Y { get; set; }
    public static implicit operator MapPosition(SavedMapPosition pos) => new() { x = pos.X, y = pos.Y };
    public static implicit operator SavedMapPosition(MapPosition pos) => new() { X = pos.x, Y = pos.y };
}
