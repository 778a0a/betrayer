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

    private const string SaveDataKeyPrefix = "SaveData";
    private static string SaveDataKey(int slotNo) => $"{SaveDataKeyPrefix}{slotNo}";
    public const int AutoSaveDataSlotNo = -1;

    public bool HasSaveData(int slotNo) => PlayerPrefs.HasKey(SaveDataKey(slotNo));
    public bool HasAutoSaveData() => PlayerPrefs.HasKey(SaveDataKey(AutoSaveDataSlotNo));

    public void Save(int slotNo, GameCore core) => Save(slotNo, CreateSaveDataText(core));
    public void Save(int slotNo, SaveDataText saveDataText)
    {
        var compressed = saveDataText.Compress();
        Debug.Log($"セーブデータ圧縮: {saveDataText.Length} -> {compressed.Length} ({compressed.Length / (float)saveDataText.Length * 100}%)");
        PlayerPrefs.SetString(SaveDataKey(slotNo), compressed);
        Debug.Log(saveDataText);
    }

    public void SaveToClipboard(GameCore core)
    {
        var saveData = CreateSaveDataText(core);
        GUIUtility.systemCopyBuffer = saveData.PlainText();
        Debug.Log(saveData);
    }

    private SaveDataText CreateSaveDataText(GameCore core)
    {
        var world = core.World;
        var state = SavedGameCoreState.Create(core);
        var saveData = SaveDataText.Serialize(world, state);
        return saveData;
    }

    public SaveData Load(int slotNo)
    {
        var text = LoadSaveDataText(slotNo);
        var saveData = text.Deserialize();
        return saveData;
    }

    public SaveDataSummary LoadSummary(int slotNo)
    {
        var text = LoadSaveDataText(slotNo);
        var summary = text.DeserializeSummary();
        return summary;
    }

    public SaveDataText LoadFromClipboard()
    {
        var text = SaveDataText.FromPlainText(GUIUtility.systemCopyBuffer);
        return text;
    }

    public void Delete(int slotNo)
    {
        PlayerPrefs.DeleteKey(SaveDataKey(slotNo));
    }

    public void Copy(int srcSlotNo, int dstSlotNo)
    {
        var textOriginal = LoadSaveDataText(srcSlotNo);
        var saveData = textOriginal.Deserialize();
        
        // スロット番号を書き換える。
        saveData.State.SaveDataSlotNo = dstSlotNo;

        var text = SaveDataText.Serialize(saveData);
        var compressed = text.Compress();
        PlayerPrefs.SetString(SaveDataKey(dstSlotNo), compressed);
    }

    public SaveDataText LoadSaveDataText(int slotNo)
    {
        var compressed = PlayerPrefs.GetString(SaveDataKey(slotNo));
        var saveData = SaveDataText.FromCompressed(compressed);
        return saveData;
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
