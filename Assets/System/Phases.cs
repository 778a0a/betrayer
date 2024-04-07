using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 収入フェイズ
/// </summary>
public class IncomePhase : PhaseBase
{
    public override void Phase()
    {
        Debug.Log("[収入フェイズ] 開始");
        // 国毎の処理を行う。
        foreach (var country in Countries)
        {
            // 支配領域数に応じて収入を得る。
            var totalIncome = country.Areas.Count * 10;
            Debug.Log($"[収入フェイズ] {country.Id} 総収入: {totalIncome}");
            var remainingIncome = totalIncome;
            // 各キャラに給料を支払う。
            foreach (var chara in country.Vassals)
            {
                var salary = totalIncome * chara.SalaryRatio / 100;
                chara.Gold += salary;
                remainingIncome -= salary;
                Debug.Log($"[収入フェイズ] {country.Id} {chara.Name} {chara.Gold} (+{salary})");
            }
            country.Ruler.Gold += remainingIncome;
            Debug.Log($"[収入フェイズ] {country.Id} {country.Ruler.Name} {country.Ruler.Gold} (+{remainingIncome})");
        }

        // 未所属の処理を行う。
        Debug.Log($"[収入フェイズ] 未所属の処理を行います。");
        var freeCharas = Characters.Where(IsFree).ToArray();
        foreach (var chara in freeCharas)
        {
            var income = Random.Range(0, 5);
            chara.Gold += income;
            Debug.Log($"[収入フェイズ] {chara.Name} {chara.Gold} (+{income})");
        }
        Debug.Log("[収入フェイズ] 終了");
    }
}

/// <summary>
/// 個人フェイズ
/// </summary>
public class PersonalActionPhase : PhaseBase
{
    public override void Phase()
    {
        Debug.Log("[個人フェイズ] 開始");
        // ランダムな順番で行動させる。
        var charas = Characters.OrderBy(c => Random.value).ToArray();
        for (int i = 0; i < charas.Length; i++)
        {
            var chara = charas[i];
            Debug.Log($"[個人フェイズ] {chara.Name} の行動を開始します。G:{chara.Gold} ({i + 1}/{charas.Length})");
            // プレイヤーの場合
            if (chara.IsPlayer)
            {
                Debug.Log($"[個人フェイズ] プレイヤーのターン");
            }
            // NPCの場合
            else
            {
                // 兵が雇えるなら雇う。
                while (PersonalActions.HireSoldier.CanDo(chara))
                {
                    PersonalActions.HireSoldier.Do(chara);
                    Debug.Log($"[個人フェイズ] 兵を雇いました。(残りG:{chara.Gold})");
                }
                // 訓練できるなら訓練する。
                while (PersonalActions.TrainSoldiers.CanDo(chara))
                {
                    // 君主の場合、戦略フェイズで行動するために多少お金を残しておく。
                    if (IsRuler(chara))
                    {
                        if (chara.Gold < 15) break;
                    }

                    PersonalActions.TrainSoldiers.Do(chara);
                    Debug.Log($"[個人フェイズ] 兵を訓練しました。(残りG:{chara.Gold})");
                }
            }
        }
        Debug.Log("[個人フェイズ] 終了");
    }
}

/// <summary>
/// 戦略フェイズ
/// </summary>
public class StrategyActionPhase : PhaseBase
{
    public override void Phase()
    {
        Debug.Log("[戦略フェイズ] 開始");
        // ランダムな順番で行動させる。
        var charas = Characters.Where(c => !IsFree(c)).OrderBy(c => Random.value).ToArray();
        for (int i = 0; i < charas.Length; i++)
        {
            var chara = charas[i];

            var country = World.CountryOf(chara);
            // プレイヤーの場合
            if (chara.IsPlayer)
            {
                Debug.Log($"[戦略フェイズ] プレイヤーのターン");
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
                    Debug.Log($"[戦略フェイズ] 君主 {chara.Name} の行動を開始します。G: {chara.Gold}");

                    // 配下が足りていないなら配下を雇う。
                    while (StrategyActions.HireVassalRandomly.CanDo(chara, World))
                    {
                        StrategyActions.HireVassalRandomly.Do(chara, World);
                        Debug.Log($"[戦略フェイズ] 配下を雇いました。(配下数: {country.Vassals.Count}) (残りG:{chara.Gold})");
                    }
                    // 配下が多すぎるなら解雇する。
                    while (StrategyActions.FireVassalMostWeak.CanDo(chara, World))
                    {
                        var isOver = country.Vassals.Count > country.VassalCountMax;
                        if (!isOver) break;
                        StrategyActions.FireVassalMostWeak.Do(chara, World);
                        Debug.Log($"[戦略フェイズ] 配下を解雇しました。(配下数: {country.Vassals.Count}) (残りG:{chara.Gold})");
                    }
                    // 侵攻する。
                    while (StrategyActions.AttackRandomly.CanDo(chara, World))
                    {
                        // 防衛可能なメンバーが少ないなら侵攻しない。
                        var freeMembers = country.Members.Where(c => !c.IsAttacked).Count();
                        if (freeMembers <= 1)
                        {
                            break;
                        }

                        Debug.Log($"[戦略フェイズ] 侵攻します。");
                        StrategyActions.AttackRandomly.Do(chara, World);
                    }
                }
                // 配下の場合
                else
                {
                    // 忠誠度が一定以下なら、一定確率で反乱を起こす。
                }
            }
        }
        Debug.Log("[戦略フェイズ] 終了");
    }
}

/// <summary>
/// 終了フェイズ
/// </summary>
public class EndPhase : PhaseBase
{
    public override void Phase()
    {
        for (int i = 0; i < Characters.Length; i++)
        {
            var chara = Characters[i];
            chara.IsAttacked = false;
            foreach (var s in chara.Force.Soldiers)
            {
                s.Hp = s.MaxHp;
            }
        }
    }
}

public class PhaseBase
{
    public WorldData World { get; set; }
    public Character[] Characters => World.Characters;
    public List<Country> Countries => World.Countries;
    public MapGrid Map => World.Map;
    public bool IsRuler(Character chara) => World.IsRuler(chara);
    public bool IsVassal(Character chara) => World.IsVassal(chara);
    public bool IsFree(Character chara) => World.IsFree(chara);

    public virtual void Phase() { }
}

public class PhaseManager
{
    public readonly IncomePhase Income = new();
    public readonly PersonalActionPhase PersonalAction = new();
    public readonly StrategyActionPhase StrategyAction = new();
    public readonly EndPhase End = new();

    public PhaseBase[] Phases => new PhaseBase[] { Income, PersonalAction, StrategyAction, End };
    public PhaseManager(WorldData world)
    {
        foreach (var phase in Phases)
        {
            phase.World = world;
        }
    }

}