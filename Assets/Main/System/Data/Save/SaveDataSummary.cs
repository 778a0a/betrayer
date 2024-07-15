using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class SaveDataSummary
{
    public int FaceImageId { get; set; }
    public string Title { get; set; }
    public string Name { get; set; }
    public int SoldierCount { get; set; }
    public int Gold { get; set; }
    public int TurnCount { get; set; }
    public int SaveDataSlotNo { get; set; }
    public DateTime SavedTime { get; set; }

    public static SaveDataSummary Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<SaveDataSummary>(json);
    }

    public static string Serialize(SaveDataSummary summary)
    {
        return JsonConvert.SerializeObject(summary);
    }

    public static SaveDataSummary Create(
        WorldData world,
        SavedGameCoreState state,
        int saveDataSlotNo,
        LocalizationManager L,
        DateTime savedTime = default)
    {
        savedTime = savedTime == default ? DateTime.Now : savedTime;
        var chara = world.Characters.FirstOrDefault(c => c.IsPlayer) ?? world.Characters.First();
        var summary = new SaveDataSummary
        {
            FaceImageId = chara.Id,
            Title = chara.GetTitle(world, L),
            Name = chara.Name,
            SoldierCount = chara.Force.SoldierCount,
            Gold = chara.Gold,
            TurnCount = state.TurnCount,
            SaveDataSlotNo = saveDataSlotNo,
            SavedTime = savedTime,
        };
        return summary;
    }
}
