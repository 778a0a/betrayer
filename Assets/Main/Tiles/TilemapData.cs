using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilemapData : ScriptableObject
{
    public const int Width = 8;
    public const int Height = 8;
    public int[] countryTileIndex = new int[Width * Height];
}
