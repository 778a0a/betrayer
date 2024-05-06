using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 国
/// </summary>
public class Country
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 君主
    /// </summary>
    public Character Ruler { get; set; }
    /// <summary>
    /// 配下
    /// </summary>
    public List<Character> Vassals { get; set; }
    /// <summary>
    /// 領地
    /// </summary>
    public List<Area> Areas { get; set; }
    /// <summary>
    /// 同盟国
    /// </summary>
    public Country Ally { get; set; }

    /// <summary>
    /// マップの国の色のインデックス
    /// </summary>
    public int ColorIndex { get; set; }

    /// <summary>
    /// 雇える配下の最大数
    /// </summary>
    public int VassalCountMax => Math.Clamp((int)Math.Ceiling(Areas.Count / 2f), 2, 8);

    public IEnumerable<Character> Members => new[] { Ruler }.Concat(Vassals.ToArray());

    public override string ToString() => $"Country ID:{Id} ({Areas.Count} areas) {Ruler.Name}";

    /// <summary>
    /// 配下を追加して給料配分を調整します。
    /// </summary>
    public void AddVassal(Character target)
    {
        // 最新の給料配分を反映する。
        Ruler.SalaryRatio = 100 - Vassals.Sum(v => v.SalaryRatio);

        // 配下にする。
        var existingMembers = Members;
        Vassals.Add(target);
        target.Contribution /= 10;

        // 給料配分を設定する。
        const int MinimumRatio = 10;
        target.SalaryRatio = MinimumRatio;
        // 既存の配下の配分を調整する。
        var remainingRatio = target.SalaryRatio;
        while (remainingRatio > 0)
        {
            foreach (var member in existingMembers)
            {
                if (member.SalaryRatio <= MinimumRatio) continue;
                member.SalaryRatio--;
                remainingRatio--;
                if (remainingRatio <= 0) break;
            }
        }
        RecalculateSalary();
    }

    public void RecalculateSalary()
    {
        Ruler.SalaryRatio = 100 - Vassals.Sum(v => v.SalaryRatio);
        foreach (var vassal in Vassals)
        {
            var index = Vassals.IndexOf(vassal);
            vassal.Loyalty = Mathf.Clamp(
                vassal.LoyaltyBase + vassal.SalaryRatio + Vassals.Count - index - vassal.Urami,
                0, 100);
        }
    }
}
