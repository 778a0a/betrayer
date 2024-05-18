using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

/// <summary>
/// 収入フェイズ
/// </summary>
public class IncomePhase : PhaseBase
{
    public override async Awaitable Phase()
    {
        Debug.Log("[収入フェイズ] 開始");
        // 国毎の処理を行う。
        foreach (var country in Countries)
        {
            // 支配領域数に応じて収入を得る。
            var totalIncome = country.TotalIncome;

            var sb = new StringBuilder();
            sb.Append($"[収入フェイズ] {country.Id} 総収入:{totalIncome}|");
            var remainingIncome = totalIncome;
            // 各キャラに給料を支払う。
            foreach (var chara in country.Vassals)
            {
                var salary = totalIncome * chara.SalaryRatio / 100;
                chara.Gold += salary;
                remainingIncome -= salary;
                sb.Append($"{chara.Name}:{chara.Gold}(+{salary})|");
            }
            country.Ruler.Gold += remainingIncome;
            sb.Append($"{country.Ruler.Name}:{country.Ruler.Gold}(+{remainingIncome})");
            Debug.Log(sb.ToString());
        }

        // 未所属の処理を行う。
        var sb2 = new StringBuilder();
        sb2.Append("[収入フェイズ] 未所属|");
        var freeCharas = Characters.Where(IsFree).ToArray();
        foreach (var chara in freeCharas)
        {
            var income = Random.Range(0, 5);
            chara.Gold += income;
            sb2.Append($"{chara.Name}:{chara.Gold}(+{income})|");
        }
        Debug.Log(sb2.ToString());
        Debug.Log("[収入フェイズ] 終了");
    }
}