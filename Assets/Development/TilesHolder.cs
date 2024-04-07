using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TilesHolder", menuName = "TilesHolder", order = 1)]
public class TilesHolder : ScriptableObject
{
    public Tile[] countries;
    public Tile[] connections;
}
