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
/// 戦略フェイズ
/// </summary>
public class StrategyActionPhase : PhaseBase
{
    public override async Awaitable Phase()
    {
        Test.OnEnterStrategyPhase();

        Debug.Log("[戦略フェイズ] 開始");
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
                    Debug.Log($"[戦略フェイズ] プレイヤーのターン");
                    Test.OnTickStrategyPhase(chara);
                    await Test.WaitUserInteraction();
                }
                // NPCの場合
                else
                {
                    Debug.Log($"[戦略フェイズ] 君主 {chara.Name} の行動を開始します。G: {chara.Gold}");

                    // 配下が足りていないなら配下を雇う。
                    while (StrategyActions.HireVassal.CanDo(chara) && 0.5.Chance())
                    {
                        await StrategyActions.HireVassal.Do(chara);
                        Debug.Log($"[戦略フェイズ] 配下を雇いました。(配下数: {country.Vassals.Count}) (残りG:{chara.Gold})");
                    }
                    // 配下が多すぎるなら解雇する。
                    while (StrategyActions.FireVassal.CanDo(chara))
                    {
                        var isOver = country.Vassals.Count > country.VassalCountMax;
                        if (!isOver) break;
                        await StrategyActions.FireVassal.Do(chara);
                        Debug.Log($"[戦略フェイズ] 配下を解雇しました。(配下数: {country.Vassals.Count}) (残りG:{chara.Gold})");
                    }
                    // 同盟する。
                    if (StrategyActions.Ally.CanDo(chara) && 0.10.Chance())
                    {
                        await StrategyActions.Ally.Do(chara);
                        Debug.Log($"[戦略フェイズ] 同盟しました。相手: {country.Ally}");
                    }

                    // 給料配分を調整する。
                    if (StrategyActions.Organize.CanDo(chara))
                    {
                        await StrategyActions.Organize.Do(chara);
                        Debug.Log($"[戦略フェイズ] 給料配分を調整しました。");
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

                }
                // NPCの場合
                else
                {
                    // 忠誠度が一定以下なら、一定確率で反乱を起こす。
                }
            }
        }

        foreach (var country in Countries)
        {
            GameCore.Instance.Tilemap.SetExhausted(country, false);
        }
        Debug.Log("[戦略フェイズ] 終了");
    }
}