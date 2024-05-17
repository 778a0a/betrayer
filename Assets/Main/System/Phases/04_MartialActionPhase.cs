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
    public override async Awaitable Phase()
    {
        Test.OnEnterMartialPhase();

        Debug.Log("[軍事フェイズ] 開始");
        // ランダムな順番で行動させる。
        var charas = Characters.Where(c => !IsFree(c)).ToArray().ShuffleInPlace();
        for (int i = 0; i < charas.Length; i++)
        {
            var chara = charas[i];

            var country = World.CountryOf(chara);

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
                        // 防衛可能なメンバーが少ないなら侵攻しない。
                        var freeMembers = country.Members.Where(c => !c.IsAttacked).Count();
                        if (freeMembers <= 1)
                        {
                            break;
                        }

                        Debug.Log($"[軍事フェイズ] 侵攻します。");
                        await MartialActions.Attack.Do(chara);
                    }
                }
                GameCore.Instance.Tilemap.SetExhausted(country, true);
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

        foreach (var country in Countries)
        {
            GameCore.Instance.Tilemap.SetExhausted(country, false);
        }
        Debug.Log("[軍事フェイズ] 終了");
    }
}