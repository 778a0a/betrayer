using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

/// <summary>
/// 軍事フェイズ
/// </summary>
public class MartialActionPhase : PhaseBase
{
    public override async ValueTask Phase()
    {
        Test.OnEnterMartialPhase();

        Debug.Log("[軍事フェイズ] 開始");
        // ランダムな順番で行動させる。
        var charas = Characters.Where(c => !IsFree(c)).ToArray().ShuffleInPlace();
        for (int i = 0; i < charas.Length; i++)
        {
            var chara = charas[i];
            var country = World.CountryOf(chara);
            Core.Tilemap.SetActiveCountry(country);

            // 君主の場合
            if (IsRuler(chara))
            {
                // プレイヤーの場合
                if (chara.IsPlayer)
                {
                    Debug.Log($"[軍事フェイズ] プレイヤーのターン");
                    Test.OnTickMartialPhase(chara);
                    await Test.WaitUserInteraction();
                }
                // NPCの場合
                else
                {
                    Debug.Log($"[軍事フェイズ] 君主 {chara.Name} の行動を開始します。G: {chara.Gold}");

                    // 侵攻する。
                    while (MartialActions.Attack.CanDo(chara))
                    {
                        var freeMembers = country.Members.Where(c => !c.IsAttacked).Count();
                        
                        // 1+未行動の周辺国数分の防衛メンバーを残す。
                        var neighbors = World.Neighbors(country);
                        var waitingCountryCount = neighbors
                            .Where(n => !n.IsExhausted)
                            .Count();
                        if (freeMembers <= waitingCountryCount + 1)
                        {
                            break;
                        }

                        Debug.Log($"[軍事フェイズ] 侵攻します。");
                        await MartialActions.Attack.Do(chara);
                        Core.Tilemap.SetActiveCountry(country);
                    }
                }
                country.IsExhausted = true;
                Core.Tilemap.SetExhausted(country, true);
            }
            // 配下の場合
            else
            {
                // プレイヤーの場合
                if (chara.IsPlayer)
                {
                    Test.OnTickMartialPhase(chara);
                    Debug.Log($"[軍事フェイズ] プレイヤーのターン");
                    await Test.WaitUserInteraction();
                }
                // NPCの場合
                else
                {
                }
            }
        }

        foreach (var c in World.Countries) c.IsExhausted = false;
        Core.Tilemap.ResetActiveCountry();
        Core.Tilemap.ResetExhausted();
        Debug.Log("[軍事フェイズ] 終了");
    }
}