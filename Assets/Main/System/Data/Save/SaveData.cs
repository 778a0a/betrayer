using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class SaveData
{
    public static string DefaultCsvPath => "Scenarios/01/character_data";
    public static string DefaultJsonPath => "Scenarios/01/country_data";

    public List<SavedCharacter> Characters { get; set; }
    public SavedCountries Countries { get; set; }
    public SavedGameCoreState State { get; set; }
    public SaveDataSummary Summary { get; set; }

    public static WorldData LoadDefaultWorldData(TilemapHelper tilemapHelper)
    {
        var csv = Resources.Load<TextAsset>(DefaultCsvPath).text;
        var json = Resources.Load<TextAsset>(DefaultJsonPath).text;
        var charas = SavedCharacters.Deserialize(csv);
        var countries = SavedCountries.Deserialize(json);
        var saveData = new SaveData
        {
            Characters = charas,
            Countries = countries,
        };
        return saveData.RestoreWorldData(tilemapHelper);
    }

    public static void SaveDefaultWorldData(WorldData world)
    {
        var charas = SavedCharacters.Extract(world);
        var csv = SavedCharacter.CreateCsv(charas);
        var csvPath = Path.Combine("Resources", DefaultCsvPath + ".csv");
        Directory.CreateDirectory(Path.GetDirectoryName(csvPath));
        File.WriteAllText(csvPath, csv);

        var countries = SavedCountries.Extract(world);
        var json = SavedCountries.Serialize(countries);
        var jsonPath = Path.Combine("Resources", DefaultJsonPath + ".json");
        Directory.CreateDirectory(Path.GetDirectoryName(jsonPath));
        File.WriteAllText(jsonPath, json);
    }

    public WorldData RestoreWorldData(TilemapHelper tilemapHelper)
    {
        var map = DefaultData.CreateMapGrid(TilemapData.Width, tilemapHelper);
        var world = new WorldData();
        world.Map = map;
        world.Characters = Characters.Select(c => c.Character).ToArray();
        world.Countries = new List<Country>();
        
        for (int i = 0; i < Countries.Countries.Count; i++)
        {
            var c = Countries.Countries[i];
            var country = new Country
            {
                Id = c.Id,
                ColorIndex = c.ColorIndex,
                IsExhausted = c.IsExhausted,
                TurnCountToDisableAlliance = c.TurnCountToDisableAlliance,
                WantsToContinueAlliance = c.WantsToContinueAlliance,
                Areas = c.Areas.Select(a => map.GetArea(a)).ToList(),
                Ruler = Characters.First(ch => ch.CountryId == c.Id && ch.IsRuler).Character,
                Vassals = Characters
                    .Where(ch => ch.CountryId == c.Id && !ch.IsRuler)
                    .OrderBy(ch => ch.MemberOrderIndex)
                    .Select(ch => ch.Character)
                    .ToList(),
            };
            world.Countries.Add(country);
        }
        for (int i = 0; i < Countries.AllyPairs.Count; i++)
        {
            var pair = Countries.AllyPairs[i];
            var a = world.Countries.First(c => c.Id == pair.id1);
            var b = world.Countries.First(c => c.Id == pair.id2);
            a.Ally = b;
            b.Ally = a;
        }

        return world;
    }
}
