using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class SaveDataSummary
{
    public int FaceImageId { get; set; }
    public string Title { get; set; }
    public string Name { get; set; }
    public int SoldierCount { get; set; }
    public int TurnCount { get; set; }
    public DateTime SavedTime { get; set; }

    public static SaveDataSummary Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<SaveDataSummary>(json);
    }

    public static string Serialize(SaveDataSummary summary)
    {
        return JsonConvert.SerializeObject(summary);
    }

    public static SaveDataSummary Create(WorldData world, SavedGameCoreState state, DateTime savedTime = default)
    {
        savedTime = savedTime == default ? DateTime.Now : savedTime;
        var chara = world.Characters.FirstOrDefault(c => c.IsPlayer) ?? world.Characters.First();
        var summary = new SaveDataSummary
        {
            FaceImageId = chara.Id,
            Title = chara.GetTitle(world),
            Name = chara.Name,
            SoldierCount = chara.Force.Soldiers.Sum(s => s.Hp),
            TurnCount = state.TurnCount,
            SavedTime = savedTime,
        };
        return summary;
    }
}
