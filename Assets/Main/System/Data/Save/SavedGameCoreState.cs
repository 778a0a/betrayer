using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
