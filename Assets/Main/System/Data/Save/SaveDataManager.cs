using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.Tilemaps;

public class SaveDataManager
{
    public static SaveDataManager Instance { get; private set; } = new();

    private const string SaveDataKey = "SaveData";

    public bool HasSaveData()
    {
        return PlayerPrefs.HasKey(SaveDataKey);
    }

    public void SaveToPlayerPref(GameCore core)
    {
        var saveData = CreateSaveDataText(core);
        var compressed = Util.CompressGzipBase64(saveData);
        Debug.Log($"セーブデータ圧縮: {saveData.Length} -> {compressed.Length} ({compressed.Length / (float)saveData.Length * 100}%)");
        PlayerPrefs.SetString(SaveDataKey, compressed);
        Debug.Log(saveData);
    }

    public void SaveToClipboard(GameCore core)
    {
        var saveData = CreateSaveDataText(core);
        GUIUtility.systemCopyBuffer = saveData;
        Debug.Log(saveData);
    }

    private string CreateSaveDataText(GameCore core)
    {
        var world = core.World;
        var state = SavedGameCoreState.Create(core);
        var saveData = SaveData.SerializeSaveData(world, state);
        return saveData;
    }


    public WorldAndState LoadFromPlayerPref(TilemapHelper tilemapHelper)
    {
        var compressed = PlayerPrefs.GetString(SaveDataKey);
        var saveData = Util.DecompressGzipBase64(compressed);
        var ws = SaveData.DeserializeSaveData(saveData, tilemapHelper);
        return ws;
    }

    public WorldAndState LoadFromClipboard(TilemapHelper tilemapHelper)
    {
        var saveData = GUIUtility.systemCopyBuffer;
        var ws = SaveData.DeserializeSaveData(saveData, tilemapHelper);
        return ws;
    }
}

public class WorldAndState
{
    public WorldAndState(WorldData world, SavedGameCoreState state)
    {
        World = world;
        State = state;
    }

    public WorldData World { get; set; }
    public SavedGameCoreState State { get; set; }
}
