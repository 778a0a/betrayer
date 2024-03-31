using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

/// <summary>
/// 収入フェイズ
/// </summary>
public class IncomePhase : PhaseBase
{
    public override void Phase()
    {
        // 国毎の処理を行う。
        foreach (var country in Countries)
        {
            // 支配領域数に応じて収入を得る。
            var totalIncome = country.Areas.Count * 10;
            var remainingIncome = totalIncome;
            // 各キャラに給料を支払う。
            foreach (var chara in country.Vassals)
            {
                var salary = totalIncome * chara.SalaryRatio / 100;
                chara.Gold += salary;
                remainingIncome -= salary;
            }
            country.Ruler.Gold += remainingIncome;
        }

        // 未所属の処理を行う。
        var freeCharas = Characters.Where(IsFree).ToArray();
        foreach (var chara in freeCharas)
        {
            chara.Gold += Random.Range(0, 5);
        }
    }
}

/// <summary>
/// 個人フェイズ
/// </summary>
public class PersonalActionPhase : PhaseBase
{
    public override void Phase()
    {
        // ランダムな順番で行動させる。
        var charas = Characters.OrderBy(c => Random.value).ToArray();
        for (int i = 0; i < charas.Length; i++)
        {
            var chara = charas[i];
            // プレイヤーの場合
            if (chara.IsPlayer)
            {
            }
            // NPCの場合
            else
            {
                // 兵が雇えるなら雇う。
                while (PersonalActions.HireSoldier.CanDo(chara))
                {
                    PersonalActions.HireSoldier.Do(chara);
                }
                // 訓練できるなら訓練する。
                while (PersonalActions.TrainSoldiers.CanDo(chara))
                {
                    PersonalActions.TrainSoldiers.Do(chara);
                }
            }
        }
    }
}

/// <summary>
/// 戦略フェイズ
/// </summary>
public class StrategyActionPhase : PhaseBase
{
    public override void Phase()
    {
        // ランダムな順番で行動させる。
        var charas = Characters.Where(c => !IsFree(c)).OrderBy(c => Random.value).ToArray();
        for (int i = 0; i < charas.Length; i++)
        {
            var chara = charas[i];
            // プレイヤーの場合
            if (chara.IsPlayer)
            {
                if (IsRuler(chara))
                {

                }
                else
                {
                }
            }
            // NPCの場合
            else
            {
                // 君主の場合
                if (IsRuler(chara))
                {
                    // 配下を雇う
                    // 侵攻する
                    // 解雇する
                    // 懲罰攻撃する
                    // 同盟を結ぶ
                }
                // 配下の場合
                else
                {
                    // 忠誠度が一定以下なら、一定確率で反乱を起こす。
                }
            }
        }
    }
}



public class PhaseBase
{
    public WorldData World { get; }
    public Character[] Characters => World.Characters;
    public List<Country> Countries => World.Countries;
    public MapGrid Map => World.Map;
    public bool IsRuler(Character chara) => World.IsRuler(chara);
    public bool IsVassal(Character chara) => World.IsVassal(chara);
    public bool IsFree(Character chara) => World.IsFree(chara);

    public virtual void Phase() { }
}