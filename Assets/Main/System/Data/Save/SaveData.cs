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
        return LoadWorldData(tilemapHelper, csv, json);
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
    public static (WorldData, SavedGameCoreState) DeserializeSaveData(
        string saveData,
        TilemapHelper tilemapHelper)
    {
        var sections = saveData.Split(new[] { SaveDataSectionDivider }, StringSplitOptions.RemoveEmptyEntries);

        var csv = sections[0].Trim();
        var json = sections[1].Trim();
        var stateJson = sections[2].Trim();
        var world = LoadWorldData(tilemapHelper, csv, json);
        var state = JsonConvert.DeserializeObject<SavedGameCoreState>(stateJson);
        return (world, state);
    }

    public static string SerializeSaveData(WorldData world, SavedGameCoreState state)
    {
        var sb = new System.Text.StringBuilder();
        
        var csv = SavedCharacters.Serialize(world);
        sb.AppendLine(csv);

        sb.AppendLine(SaveDataSectionDivider);
        var json = SavedCountries.Serialize(world);
        sb.AppendLine(json);

        sb.AppendLine(SaveDataSectionDivider);
        var stateJson = JsonConvert.SerializeObject(state);
        sb.AppendLine(stateJson);

        return sb.ToString();
    }

    public static WorldData LoadWorldData(
        TilemapHelper tilemapHelper,
        string csv,
        string json)
    {
        var map = DefaultData.CreateMapGrid(TilemapData.Width, tilemapHelper);
        var world = new WorldData();
        var charas = SavedCharacters.Deserialize(csv);
        var countries = SavedCountries.Deserialize(json);
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
