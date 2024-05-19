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
            Core.Tilemap.SetExhausted(country, false);
        }

        if (Countries.Count == 2)
        {
            if (Countries[0].Ally != null)
            {
                Countries[0].Ally = null;
                Countries[1].Ally = null;
                Debug.Log($"[開始フェイズ] 残り勢力数が2になったため、同盟が解消されました。");
            }
        }
        Debug.Log("[開始フェイズ] 終了");
    }
}
