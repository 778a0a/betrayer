using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapEditorWindow : EditorWindow
{
    //private TilemapData selectedData;
    private SerializedObject serializedObject;
    private SerializedProperty tilesProperty;

    private string filename;
    private string saveDirectory = "Assets/Development/SavedMaps";

    [MenuItem("開発/Tilemap State Manager")]
    public static void ShowWindow()
    {
        var window = GetWindow<TilemapEditorWindow>();
        window.titleContent = new GUIContent("Tilemap State Manager");
        window.Show();
    }

    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);
    }

    private void OnGUI()
    {
        GUILayout.Label("名前をつけて保存", EditorStyles.boldLabel);
        filename = GUILayout.TextField(filename);
        if (GUILayout.Button("保存"))
        {
            SaveTilemapState(filename);
        }

        GUILayout.Space(10);

        GUILayout.Label("保存マップ一覧", EditorStyles.boldLabel);
        var files = Directory.GetFiles(saveDirectory, "*.asset");
        foreach (var file in files)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (GUILayout.Button(name))
            {
                LoadSelectedState(name);
            }
        }
    }

    private void SaveTilemapState(string filename)
    {
        // 保存先。
        var path = Path.Combine(saveDirectory, filename + ".asset");
        // すでにファイルがある場合は確認する。
        if (File.Exists(path))
        {
            if (!EditorUtility.DisplayDialog("確認", "ファイルがすでに存在します。上書きしますか？", "はい", "いいえ"))
            {
                return;
            }
        }

        var data = CreateInstance<TilemapData>();

        var tilesHolder = AssetDatabase.LoadAssetAtPath<TilesHolder>("Assets/Development/TilesHolder.asset");
        Debug.Log(tilesHolder.countries.Length);

        // ヒエラルキーからCountryTilemapを取得する。
        var tilemap = GameObject.Find("CountryTilemap").GetComponent<Tilemap>();
        for (int x = 0; x < TilemapData.Width; x++)
        {
            for (int y = 0; y < TilemapData.Height; y++)
            {
                var tile = tilemap.GetTile(new Vector3Int(x, -y, 0));
                if (tile != null)
                {
                    var index = Array.IndexOf(tilesHolder.countries, tile);
                    data.countryTileIndex[x + y * TilemapData.Width] = index;
                }
            }
        }

        AssetDatabase.CreateAsset(data, path);
    }

    private void LoadSelectedState(string filename)
    {
        var path = Path.Combine(saveDirectory, filename + ".asset");
        var data = AssetDatabase.LoadAssetAtPath<TilemapData>(path);

        var tilesHolder = AssetDatabase.LoadAssetAtPath<TilesHolder>("Assets/Development/TilesHolder.asset");
        var tilemap = GameObject.Find("CountryTilemap").GetComponent<Tilemap>();
        for (int x = 0; x < TilemapData.Width; x++)
        {
            for (int y = 0; y < TilemapData.Height; y++)
            {
                var index = data.countryTileIndex[x + y * TilemapData.Width];
                if (index >= 0)
                {
                    tilemap.SetTile(new Vector3Int(x, -y, 0), tilesHolder.countries[index]);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, -y, 0), null);
                }
            }
        }
    }
}
