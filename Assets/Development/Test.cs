using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap tilemap;

    [SerializeField] private Tile[] countryTiles;
    [SerializeField] private Tile[] connectionTiles;

    // Start is called before the first frame update
    void Start()
    {
        tilemap.SetTile(new Vector3Int(0, 0, 0), countryTiles[20]);
        tilemap.SetTile(new Vector3Int(1, 0, 0), countryTiles[20]);
        tilemap.SetTile(new Vector3Int(2, 0, 0), countryTiles[20]);
        tilemap.SetTile(new Vector3Int(0, -1, 0), countryTiles[20]);
        tilemap.SetTile(new Vector3Int(0, -2, 0), countryTiles[20]);
        tilemap.SetTile(new Vector3Int(0, -3, 0), countryTiles[20]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
