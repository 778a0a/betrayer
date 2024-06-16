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
    public static string CsvPath => "Scenarios/01/character_data";
    public static string JsonPath => "Scenarios/01/country_data";


    public static WorldData LoadWorldData(TilemapHelper tilemapHelper)
    {
        var map = DefaultData.CreateMapGrid(TilemapData.Width, tilemapHelper);

        var world = new WorldData();

        var csv = Resources.Load<TextAsset>(CsvPath).text;
        var charas = SavedCharacter.LoadCharacterData(csv);
        var json = Resources.Load<TextAsset>(JsonPath).text;
        var countries = JsonConvert.DeserializeObject<SavedCountries>(json);
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
        return world;
    }

    public static void SaveWorldData(WorldData world)
    {
        SaveCharacterData(world);
        SaveCountryData(world);
    }

    public static void SaveCharacterData(WorldData world)
    {
        var charas = new List<SavedCharacter>();
        for (int i = 0; i < world.Characters.Length; i++)
        {
            var character = world.Characters[i];
            var country = world.Countries.FirstOrDefault(c => c.Members.Contains(character));
            var memberIndex = country?.Members.TakeWhile(c => c != character).Count() ?? -1;
            var chara = new SavedCharacter
            {
                Character = character,
                CountryId = country != null ? country.Id : -1,
                MemberOrderIndex = memberIndex,
            };
            charas.Add(chara);
        }
        charas = charas.OrderBy(c => c.CountryId).ThenBy(c => c.MemberOrderIndex).ToList();

        var csv = SavedCharacter.CreateCsv(charas);
        var path = Path.Combine("Resources", CsvPath + ".csv");
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, csv);
    }

    public static void SaveCountryData(WorldData world)
    {
        var countries = new SavedCountries
        {
            Countries = world.Countries.Select(c => new SavedCountry
            {
                Id = c.Id,
                ColorIndex = c.ColorIndex,
                Areas = c.Areas.Select(a => (SavedMapPosition)a.Position).ToList(),
            }).ToList(),
        };
        var json = JsonConvert.SerializeObject(countries);
        var path = Path.Combine("Resources", JsonPath + ".json");
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, json);
    }
}

public class SavedCountries
{
    public List<SavedCountry> Countries { get; set; }
}
public class SavedCountry
{
    public int Id { get; set; }
    public int ColorIndex { get; set; }
    public List<SavedMapPosition> Areas { get; set; }
}
public class SavedMapPosition
{
    public int X { get; set; }
    public int Y { get; set; }
    public static implicit operator MapPosition(SavedMapPosition pos) => new() { x = pos.X, y = pos.Y };
    public static implicit operator SavedMapPosition(MapPosition pos) => new() { X = pos.x, Y = pos.y };
}


public class SavedCharacter
{
    public int CountryId { get; set; }
    public int MemberOrderIndex { get; set; }
    public Character Character { get; set; }

    public bool IsRuler => !IsFree && MemberOrderIndex == 0;
    public bool IsFree => CountryId == -1;

    public static string CreateCsv(List<SavedCharacter> charas)
    {
        var json = JsonConvert.SerializeObject(charas);
        var list = JsonConvert.DeserializeObject<List<JObject>>(json);
        var sb = new System.Text.StringBuilder();

        var delimiter = "\t";
        // ヘッダー
        sb.Append(nameof(CountryId)).Append(delimiter);
        sb.Append(nameof(MemberOrderIndex)).Append(delimiter);
        foreach (JProperty prop in list[0][nameof(Character)])
        {
            sb.Append(prop.Name).Append(delimiter);
        }
        sb.AppendLine();

        // 中身
        foreach (var obj in list)
        {
            sb.Append(obj[nameof(CountryId)]).Append(delimiter);
            sb.Append(obj[nameof(MemberOrderIndex)]).Append(delimiter);
            foreach (JProperty prop in obj[nameof(Character)])
            {
                sb.Append(JsonConvert.SerializeObject(prop.Value)).Append(delimiter);
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public static List<SavedCharacter> LoadCharacterData(string csv)
    {
        //var csv = File.ReadAllText(path);
        var lines = csv.Trim().Split('\n');
        var header = lines[0].Trim().Split('\t');
        var charas = new List<SavedCharacter>();
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }
            var values = line.Split('\t');
            var chara = new SavedCharacter
            {
                CountryId = int.Parse(values[0]),
                MemberOrderIndex = int.Parse(values[1]),
            };
            var character = new Character();
            for (int j = 2; j < header.Length; j++)
            {
                var propName = header[j];
                var prop = character.GetType().GetProperty(propName);
                // has setter
                if (prop.CanWrite)
                {
                    var type = prop.PropertyType;
                    var value = JsonConvert.DeserializeObject(values[j], type);
                    prop.SetValue(character, value);
                }
            }
            chara.Character = character;
            charas.Add(chara);
        }
        return charas;
    }
}
