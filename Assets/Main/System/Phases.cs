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
    public override IEnumerator Phase()
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
        yield break;
    }
}

/// <summary>
/// 収入フェイズ
/// </summary>
public class IncomePhase : PhaseBase
{
    public override IEnumerator Phase()
    {
        Debug.Log("[収入フェイズ] 開始");
        // 国毎の処理を行う。
        foreach (var country in Countries)
        {
            // 支配領域数に応じて収入を得る。
            var totalIncome = country.Areas.Select(IncomeOf).Sum();
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
        yield break;
    }

    private int IncomeOf(Area a) => a.Terrain switch
    {
        Terrain.Plain => 10,
        Terrain.Hill => 7,
        Terrain.Forest => 5,
        Terrain.Mountain => 3,
        Terrain.Fort => 20,
        _ => 1,
    };
}

/// <summary>
/// 個人フェイズ
/// </summary>
public class PersonalActionPhase : PhaseBase
{
    public override IEnumerator Phase()
    {
        Test.Instance.OnEnterIndividualPhase();

        Debug.Log("[個人フェイズ] 開始");
        // ランダムな順番で行動させる。
        var charas = Characters.ToArray().ShuffleInPlace();
        for (int i = 0; i < charas.Length; i++)
        {
            var chara = charas[i];
            Test.Instance.OnTickIndividualPhase(chara);

            Debug.Log($"[個人フェイズ] {chara.Name} の行動を開始します。G:{chara.Gold} ({i + 1}/{charas.Length})");
            // プレイヤーの場合
            if (chara.IsPlayer)
            {
                Debug.Log($"[個人フェイズ] プレイヤーのターン");
                Test.Instance.hold = true;
                yield return Test.Instance.HoldIfNeeded();
            }
            // NPCの場合
            else
            {
                //if (World.IsVassal(chara) && chara.Loyalty < 90)
                //{
                //    // 忠誠度が一定以下なら、一定確率で反抗的行動を行う。
                //    var percent = (90 - chara.Loyalty) / 100f;
                //    var hindo = 10;
                //    if (Random.value <  percent / hindo)
                //    {
                //    }
                //}

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
        yield break;
    }
}

/// <summary>
/// 戦略フェイズ
/// </summary>
public class StrategyActionPhase : PhaseBase
{
    public override IEnumerator Phase()
    {
        Test.Instance.OnEnterStrategyPhase();

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
                Test.Instance.OnTickStrategyPhase(chara);

                // プレイヤーの場合
                if (chara.IsPlayer)
                {
                    Debug.Log($"[戦略フェイズ] プレイヤーのターン");
                    Test.Instance.hold = true;
                    yield return Test.Instance.HoldIfNeeded();
                }
                // NPCの場合
                else
                {
                    Debug.Log($"[戦略フェイズ] 君主 {chara.Name} の行動を開始します。G: {chara.Gold}");

                    // 配下が足りていないなら配下を雇う。
                    while (StrategyActions.HireVassal.CanDo(chara) && 0.5.Chance())
                    {
                        StrategyActions.HireVassal.Do(chara);
                        Debug.Log($"[戦略フェイズ] 配下を雇いました。(配下数: {country.Vassals.Count}) (残りG:{chara.Gold})");
                    }
                    // 配下が多すぎるなら解雇する。
                    while (StrategyActions.FireVassal.CanDo(chara))
                    {
                        var isOver = country.Vassals.Count > country.VassalCountMax;
                        if (!isOver) break;
                        StrategyActions.FireVassal.Do(chara);
                        Debug.Log($"[戦略フェイズ] 配下を解雇しました。(配下数: {country.Vassals.Count}) (残りG:{chara.Gold})");
                    }
                    // 同盟する。
                    if (StrategyActions.Ally.CanDo(chara) && 0.10.Chance())
                    {
                        StrategyActions.Ally.Do(chara);
                        Debug.Log($"[戦略フェイズ] 同盟しました。相手: {country.Ally}");
                    }

                    // 給料配分を調整する。
                    if (StrategyActions.Organize.CanDo(chara))
                    {
                        StrategyActions.Organize.Do(chara);
                        Debug.Log($"[戦略フェイズ] 給料配分を調整しました。");
                    }
                }
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
        Debug.Log("[戦略フェイズ] 終了");
        yield break;
    }
}

/// <summary>
/// 軍事フェイズ
/// </summary>
public class MartialActionPhase : PhaseBase
{
    public override IEnumerator Phase()
    {
        Test.Instance.OnEnterMartialPhase();

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
                Test.Instance.OnTickMartialPhase(chara);

                // プレイヤーの場合
                if (chara.IsPlayer)
                {
                    Debug.Log($"[軍事フェイズ] プレイヤーのターン");
                    Test.Instance.hold = true;
                    yield return Test.Instance.HoldIfNeeded();
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
                        var waiting = true;

                        Test.Instance.StartCoroutine(Attack());
                        IEnumerator Attack()
                        {
                            MartialActions.Attack.Do(chara).GetAwaiter().OnCompleted(() => waiting = false);
                            yield break;
                        }

                        yield return new WaitUntil(() => !waiting);
                    }
                }
            }
            // 配下の場合
            else
            {
                // プレイヤーの場合
                if (chara.IsPlayer)
                {
                    Test.Instance.OnTickMartialPhase(chara);
                    Debug.Log($"[軍事フェイズ] プレイヤーのターン");
                    Test.Instance.hold = true;
                    yield return Test.Instance.HoldIfNeeded();
                }
                // NPCの場合
                else
                {
                }
            }
        }
        Debug.Log("[軍事フェイズ] 終了");
        yield break;
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

    public virtual IEnumerator Phase() { yield break; }
}

public class PhaseManager
{
    public readonly StartPhase Start = new();
    public readonly IncomePhase Income = new();
    public readonly StrategyActionPhase StrategyAction = new();
    public readonly PersonalActionPhase PersonalAction = new();
    public readonly MartialActionPhase MartialAction = new();

    public PhaseBase[] Phases => new PhaseBase[] { Start, Income, StrategyAction, PersonalAction, MartialAction };
    public PhaseManager(WorldData world)
    {
        foreach (var phase in Phases)
        {
            phase.World = world;
        }
    }

}