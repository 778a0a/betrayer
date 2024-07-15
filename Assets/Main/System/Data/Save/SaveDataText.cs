using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class SaveDataText
{
    private readonly static string SaveDataSectionDivider = ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>";

    private string saveDataText;
    private SaveDataText(string saveDataText)
    {
        this.saveDataText = saveDataText;
    }

    public static SaveDataText FromCompressed(string compressed)
    {
        var saveDataText = Util.DecompressGzipBase64(compressed);
        return new SaveDataText(saveDataText);
    }

    public static SaveDataText FromPlainText(string saveDataText)
    {
        return new SaveDataText(saveDataText);
    }

    public string Compress() => Util.CompressGzipBase64(saveDataText);
    public string PlainText() => saveDataText;
    public int Length => saveDataText.Length;

    public static SaveDataText Serialize(
        WorldData world,
        SavedGameCoreState state,
        int saveDataSlotNo,
        LocalizationManager L,
        DateTime savedTime = default)
    {
        var charas = SavedCharacters.Extract(world);
        var countries = SavedCountries.Extract(world);
        var summary = SaveDataSummary.Create(world, state, saveDataSlotNo, L, savedTime);
        var saveData = new SaveData
        {
            Characters = charas,
            Countries = countries,
            State = state,
            Summary = summary,
        };
        var text = Serialize(saveData);
        return text;
    }

    public static SaveDataText Serialize(SaveData data)
    {
        var sb = new System.Text.StringBuilder();

        // キャラデータ
        var charasCsv = SavedCharacter.CreateCsv(data.Characters);
        sb.AppendLine(charasCsv);

        // 国データ
        sb.AppendLine(SaveDataSectionDivider);
        var countriesJson = SavedCountries.Serialize(data.Countries);
        sb.AppendLine(countriesJson);

        // ゲーム状態
        sb.AppendLine(SaveDataSectionDivider);
        var stateJson = SavedGameCoreState.Serialize(data.State);
        sb.AppendLine(stateJson);

        // セーブ画面用情報
        sb.AppendLine(SaveDataSectionDivider);
        var summaryJson = SaveDataSummary.Serialize(data.Summary);
        sb.AppendLine(summaryJson);

        return FromPlainText(sb.ToString());
    }


    public SaveData Deserialize()
    {
        var sections = saveDataText.Split(
            new[] { SaveDataSectionDivider },
            StringSplitOptions.RemoveEmptyEntries);

        var charasCsv = sections[0].Trim();
        var charas = SavedCharacters.Deserialize(charasCsv);

        var countriesJson = sections[1].Trim();
        var countries = SavedCountries.Deserialize(countriesJson);

        var stateJson = sections[2].Trim();
        var state = SavedGameCoreState.Deserialize(stateJson);

        var summaryJson = sections[3].Trim();
        var summary = SaveDataSummary.Deserialize(summaryJson);

        return new SaveData
        {
            Characters = charas,
            Countries = countries,
            State = state,
            Summary = summary,
        };
    }

    public SaveDataSummary DeserializeSummary()
    {
        var sections = saveDataText.Split(
            new[] { SaveDataSectionDivider },
            StringSplitOptions.RemoveEmptyEntries);

        var summaryJson = sections[3].Trim();
        var summary = SaveDataSummary.Deserialize(summaryJson);

        return summary;
    }

    public override string ToString()
    {
        return saveDataText;
    }
}