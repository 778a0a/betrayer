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
/// 個人フェイズ
/// </summary>
public class PersonalActionPhase : PhaseBase
{
    public override void SetActionOrder()
    {
        ActionOrder = Characters
            .ShuffleAsArray();
    }

    public override async ValueTask Phase()
    {
        Test.OnEnterIndividualPhase();

        Debug.Log("[個人フェイズ] 開始");
        // ランダムな順番で行動させる。
        for (int i = 0; i < ActionOrder.Length; i++)
        {
            var chara = ActionOrder[i];
            if (chara.IsExhausted) continue;

            Debug.Log($"[個人フェイズ] {chara.Name} の行動を開始します。G:{chara.Gold} ({i + 1}/{ActionOrder.Length})");
            // プレイヤーの場合
            if (chara.IsPlayer)
            {
                Debug.Log($"[個人フェイズ] プレイヤーのターン");
                Test.OnTickIndividualPhase(chara);
                await Test.WaitUserInteraction();
            }
            // NPCの場合
            else
            {
                var prevGold = chara.Gold;
                var prevPower = chara.Power;
                var hireCount = 0;
                var trainedCount = 0;
                // 兵が雇えるなら雇う。
                while (PersonalActions.HireSoldier.CanDo(chara))
                {
                    await PersonalActions.HireSoldier.Do(chara);
                    hireCount++;
                }

                var canBecomeIndependent = PersonalActions.BecomeIndependent.CanDo(chara);
                var canRebel = PersonalActions.Rebel.CanDo(chara);
                if ((canRebel || canBecomeIndependent) && chara.Loyalty < 90)
                {
                    var country = World.CountryOf(chara);
                    // 忠誠度が一定以下なら、一定確率で反抗的行動を行う。
                    var percent = (90 - chara.Loyalty) / 100f;
                    var chance = percent / 40; // 40ターンに1回反乱を起こす確率がpercent
                    if (country.Ruler.IsPlayer && chara.Urami > 0) chance *= chara.Urami;
                    if (chance.Chance())
                    {
                        if (canBecomeIndependent)
                        {
                            await PersonalActions.BecomeIndependent.Do(chara);
                        }
                        else if (canRebel)
                        {
                            await PersonalActions.Rebel.Do(chara);
                        }
                    }
                }

                // 訓練できるなら訓練する。
                while (PersonalActions.TrainSoldiers.CanDo(chara))
                {
                    // 君主の場合、戦略フェイズで行動するために多少お金を残しておく。
                    if (IsRuler(chara))
                    {
                        if (chara.Gold < 15) break;
                    }

                    await PersonalActions.TrainSoldiers.Do(chara);
                    trainedCount++;
                }
                if (hireCount > 0 || trainedCount > 0)
                {
                    Debug.Log($"[個人フェイズ] " +
                        (hireCount > 0 ? $"雇兵: {hireCount} " : "") +
                        $"訓練: {trainedCount} " +
                        $"Power: {prevPower}->{chara.Power} " +
                        $"Gold:{prevGold}->{chara.Gold}");
                }
            }
            chara.IsExhausted = true;
        }

        foreach (var c in World.Characters) c.IsExhausted = false;
        Debug.Log("[個人フェイズ] 終了");
    }
}
