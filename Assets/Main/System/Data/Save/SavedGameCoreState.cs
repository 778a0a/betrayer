using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class SavedGameCoreState
{
    public int TurnCount { get; set; }
    public string CurrentPhase { get; set; }
    public int[] CurrentActionOrder { get; set; }

    public bool IsTargetPhase(PhaseBase phase)
    {
        return phase.GetType().Name == CurrentPhase;
    }

    public Character[] RestoreActionOrder(Character[] all)
    {
        var order = new Character[CurrentActionOrder.Length];
        var dict = all.ToDictionary(c => c.Id);
        for (int i = 0; i < CurrentActionOrder.Length; i++)
        {
            var id = CurrentActionOrder[i];
            order[i] = dict[id];
        }
        return order;
    }

    public static SavedGameCoreState Create(GameCore core)
    {
        var phase = core.CurrentPhase;
        return new SavedGameCoreState
        {
            TurnCount = core.TurnCount,
            CurrentPhase = phase.GetType().Name,
            CurrentActionOrder = phase.ActionOrder.Select(c => c.Id).ToArray(),
        };
    }

    public static SavedGameCoreState Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<SavedGameCoreState>(json);
    }

    public static string Serialize(SavedGameCoreState state)
    {
        return JsonConvert.SerializeObject(state);
    }
}
