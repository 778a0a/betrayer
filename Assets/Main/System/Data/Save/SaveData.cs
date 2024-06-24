using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveData
{
    public static string DefaultCsvPath => "Scenarios/01/character_data";
    public static string DefaultJsonPath => "Scenarios/01/country_data";

    public static WorldData LoadDefaultWorldData(TilemapHelper tilemapHelper)
    {
        var csv = Resources.Load<TextAsset>(DefaultCsvPath).text;
        var json = Resources.Load<TextAsset>(DefaultJsonPath).text;
        var charas = SavedCharacters.Deserialize(csv);
        var countries = SavedCountries.Deserialize(json);
        return LoadWorldData(tilemapHelper, charas, countries);
    }

    public static void SaveDefaultWorldData(WorldData world)
    {
        var csv = SavedCharacters.Serialize(world);
        var csvPath = Path.Combine("Resources", DefaultCsvPath + ".csv");
        Directory.CreateDirectory(Path.GetDirectoryName(csvPath));
        File.WriteAllText(csvPath, csv);

        var json = SavedCountries.Serialize(world);
        var jsonPath = Path.Combine("Resources", DefaultJsonPath + ".json");
        Directory.CreateDirectory(Path.GetDirectoryName(jsonPath));
        File.WriteAllText(jsonPath, json);
    }

    private readonly static string SaveDataSectionDivider = ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>";
    public static WorldAndState DeserializeSaveData(
        string saveData,
        TilemapHelper tilemapHelper)
    {
        var sections = saveData.Split(new[] { SaveDataSectionDivider }, StringSplitOptions.RemoveEmptyEntries);

        var csv = sections[0].Trim();
        var json = sections[1].Trim();
        var stateJson = sections[2].Trim();

        var charas = SavedCharacters.Deserialize(csv);
        var countries = SavedCountries.Deserialize(json);
        var world = LoadWorldData(tilemapHelper, charas, countries);
        var state = SavedGameCoreState.Deserialize(stateJson);
        return new WorldAndState(world, state);
    }

    public static SaveDataSummary DeserializeSaveDataSummary(string saveData)
    {
        var sections = saveData.Split(new[] { SaveDataSectionDivider }, StringSplitOptions.RemoveEmptyEntries);
        var json = sections[3].Trim();
        return SaveDataSummary.Deserialize(json);
    }

    public static string SerializeSaveData(WorldData world, SavedGameCoreState state, DateTime savedTime = default)
    {
        var sb = new System.Text.StringBuilder();
        
        // キャラデータ
        var csv = SavedCharacters.Serialize(world);
        sb.AppendLine(csv);

        // 国データ
        sb.AppendLine(SaveDataSectionDivider);
        var json = SavedCountries.Serialize(world);
        sb.AppendLine(json);

        // ゲーム状態
        sb.AppendLine(SaveDataSectionDivider);
        var stateJson = SavedGameCoreState.Serialize(state);
        sb.AppendLine(stateJson);

        // セーブ画面用情報
        sb.AppendLine(SaveDataSectionDivider);
        var summary = SaveDataSummary.Create(world, state, savedTime);
        var summaryJson = SaveDataSummary.Serialize(summary);
        sb.AppendLine(summaryJson);

        return sb.ToString();
    }

    public static WorldData LoadWorldData(
        TilemapHelper tilemapHelper,
        List<SavedCharacter> charas,
        SavedCountries countries)
    {
        var map = DefaultData.CreateMapGrid(TilemapData.Width, tilemapHelper);
        var world = new WorldData();
        world.Map = map;
        world.Characters = charas.Select(c => c.Character).ToArray();
        world.Countries = new List<Country>();
        
        for (int i = 0; i < countries.Countries.Count; i++)
        {
            var c = countries.Countries[i];
            var country = new Country
            {
                Id = c.Id,
                ColorIndex = c.ColorIndex,
                IsExhausted = c.IsExhausted,
                TurnCountToDisableAlliance = c.TurnCountToDisableAlliance,
                WantsToContinueAlliance = c.WantsToContinueAlliance,
                Areas = c.Areas.Select(a => map.GetArea(a)).ToList(),
                Ruler = charas.First(ch => ch.CountryId == c.Id && ch.IsRuler).Character,
                Vassals = charas
                    .Where(ch => ch.CountryId == c.Id && !ch.IsRuler)
                    .OrderBy(ch => ch.MemberOrderIndex)
                    .Select(ch => ch.Character)
                    .ToList(),
            };
            world.Countries.Add(country);
        }
        for (int i = 0; i < countries.AllyPairs.Count; i++)
        {
            var pair = countries.AllyPairs[i];
            var a = world.Countries.First(c => c.Id == pair.id1);
            var b = world.Countries.First(c => c.Id == pair.id2);
            a.Ally = b;
            b.Ally = a;
        }

        return world;
    }
}
