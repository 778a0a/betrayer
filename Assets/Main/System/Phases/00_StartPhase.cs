using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 開始フェイズ
/// </summary>
public class StartPhase : PhaseBase
{
    public override async ValueTask Phase()
    {
        Debug.Log("[開始フェイズ] 開始");

        // 各キャラの状態をリセットする。
        for (int i = 0; i < Characters.Length; i++)
        {
            var chara = Characters[i];
            chara.IsAttacked = false;
            foreach (var s in chara.Force.Soldiers)
            {
                s.Hp = s.MaxHp;
            }
        }

        // タイル状態更新
        foreach (var country in Countries)
        {
            country.IsExhausted = false;
            Core.Tilemap.SetExhausted(country, false);
        }

        // 必要なら同盟を解消する。
        if (Countries.Count == 2)
        {
            if (Countries[0].Ally != null)
            {
                Countries[0].Ally = null;
                Countries[1].Ally = null;
                Debug.Log($"[開始フェイズ] 残り勢力数が2になったため、同盟が解消されました。");
            }
        }
        // 同盟解消
        foreach (var country in Countries)
        {
            if (country.Ally == null) continue;
            var ally = country.Ally;

            // 解消意向でないなら何もしない。
            if (country.WantsToContinueAlliance) continue;

            // 相手方も解消意向なら解消する。
            if (!ally.WantsToContinueAlliance)
            {
                country.Ally = null;
                ally.Ally = null;
                Debug.Log($"[開始フェイズ] {country.Name} と {ally.Name} の同盟が解消されました。(合意による解消)");
                continue;
            }
            
            // 相手方が継続意向なら、同盟解消ターン数を更新する。
            country.TurnCountToDisableAlliance--;
            if (country.TurnCountToDisableAlliance <= 0)
            {
                country.Ally = null;
                ally.Ally = null;
                Debug.Log($"[開始フェイズ] {country.Name} と {ally.Name} の同盟が解消されました。({country.Name}からの破棄)");
            }
        }

        Debug.Log("[開始フェイズ] 終了");
    }
}
