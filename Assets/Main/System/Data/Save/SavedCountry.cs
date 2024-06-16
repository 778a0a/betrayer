using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class SavedCountries
{
    public List<SavedCountry> Countries { get; set; }
    [JsonIgnore]
    public List<(int id1, int id2)> AllyPairs { get; set; } = new();
    public List<string> AllyPairsText
    {
        get => AllyPairs?.Select(p => $"{p.id1},{p.id2}").ToList();
        set => AllyPairs = value.Select(p =>
        {
            var split = p.Split(',');
            return (int.Parse(split[0]), int.Parse(split[1]));
        }).ToList();
    }


    public static SavedCountries Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<SavedCountries>(json);
    }

    public static string Serialize(WorldData world)
    {
        var countries = new SavedCountries
        {
            Countries = world.Countries.Select(c => new SavedCountry
            {
                Id = c.Id,
                ColorIndex = c.ColorIndex,
                Areas = c.Areas.Select(a => (SavedMapPosition)a.Position).ToList(),
            }).ToList(),
            AllyPairs = world.Countries
                .Where(c => c.Ally != null)
                .Select(c => (a: c.Id, b: c.Ally.Id))
                .Select(p => p.a < p.b ? (p.a, p.b) : (p.b, p.a))
                .Distinct()
                .ToList(),
        };
        var json = JsonConvert.SerializeObject(countries);
        return json;
    }
}

public class SavedCountry
{
    public int Id { get; set; }
    public int ColorIndex { get; set; }

    [JsonIgnore]
    public List<SavedMapPosition> Areas { get; set; }

    public List<string> AreasText
    {
        get => Areas?.Select(a => (string)a).ToList();
        set => Areas = value.Select(a => (SavedMapPosition)a).ToList();
    }
}

public class SavedMapPosition
{
    public int X { get; set; }
    public int Y { get; set; }
    public static implicit operator MapPosition(SavedMapPosition pos) => new() { x = pos.X, y = pos.Y };
    public static implicit operator SavedMapPosition(MapPosition pos) => new() { X = pos.x, Y = pos.y };
    
    public static explicit operator string (SavedMapPosition pos) => $"{pos.X},{pos.Y}";
    public static explicit operator SavedMapPosition(string pos)
    {
        var split = pos.Split(',');
        return new SavedMapPosition { X = int.Parse(split[0]), Y = int.Parse(split[1]) };
    }
}
