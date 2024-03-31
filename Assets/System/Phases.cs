using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    // 君主の場合、戦略フェイズで行動するために多少お金を残しておく。
                    if (IsRuler(chara))
                    {
                        if (chara.Gold < 15) break;
                    }

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
            var country = World.CountryOf(chara);
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
                    // 配下が足りていないなら配下を雇う。
                    while (StrategyActions.HireVassalRandomly.CanDo(chara, World))
                    {
                        StrategyActions.HireVassalRandomly.Do(chara, World);
                    }
                    // 配下が多すぎるなら解雇する。
                    while (StrategyActions.FireVassalMostWeak.CanDo(chara, World))
                    {
                        var isOver = country.Vassals.Count > country.VassalCountMax;
                        if (!isOver) break;
                        StrategyActions.FireVassalMostWeak.Do(chara, World);
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
    public WorldData World { get; }
    public Character[] Characters => World.Characters;
    public List<Country> Countries => World.Countries;
    public MapGrid Map => World.Map;
    public bool IsRuler(Character chara) => World.IsRuler(chara);
    public bool IsVassal(Character chara) => World.IsVassal(chara);
    public bool IsFree(Character chara) => World.IsFree(chara);

    public virtual void Phase() { }
}
