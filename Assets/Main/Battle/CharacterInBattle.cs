using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public record CharacterInBattle(
    Character Character,
    Terrain Terrain,
    Area Area,
    bool IsAttacker)
{
    public CharacterInBattle Opponent { get; set; }
    public Country Country = GameCore.Instance.World.CountryOf(Area);
    public bool IsDefender => !IsAttacker;

    /// <summary>
    /// 戦闘の強さ
    /// 攻撃側ならAttack、防御側ならDefense
    /// </summary>
    public int Strength => IsAttacker ? Character.Attack : Character.Defense;

    public Force Force => Character.Force;
    public bool IsPlayer => Character.IsPlayer;
    public bool AllSoldiersDead => Character.Force.Soldiers.All(s => !s.IsAlive);

    public static implicit operator Character(CharacterInBattle c) => c.Character;

    public bool ShouldRetreat(int tickCount)
    {
        // プレーヤーの場合はUIで判断しているので処理不要。
        if (IsPlayer) return false;

        // 戦闘開始直後は撤退しない。
        if (tickCount < 3) return false;

        // まだ損耗が多くないなら撤退しない。
        var manyLoss = Character.Force.Soldiers.Count(s => s.Hp < 10) >= 3;
        if (!manyLoss) return false;

        // 敵よりも兵力が多いなら撤退しない。
        var myPower = Force.Power;
        var opPower = Opponent.Force.Power;
        if (myPower > opPower) return false;

        // 敵に残り数の少ない兵士がいるなら撤退しない。
        var opAboutToDie = Opponent.Force.Soldiers.Any(s => s.IsAlive && s.Hp <= 3);
        if (opAboutToDie) return false;

        // 自国の最後の領土の防衛なら撤退しない。
        var lastArea =  Country.Areas.Count == 1;
        if (lastArea) return IsDefender && false;

        // 撤退する。
        return true;
    }

    /// <summary>
    /// 戦闘後の回復処理
    /// </summary>
    public void Recover(bool win)
    {
        if (Character == null) return;

        foreach (var s in Character.Force.Soldiers)
        {
            if (!s.IsAlive) continue;

            var baseAmount = s.MaxHp * (win ? 0.1f : 0.05f);
            var adj = Mathf.Max(0, (Character.Intelligence - 80) / 100f / 2);
            var amount = (int)(baseAmount * (1 + adj));
            s.Hp = Mathf.Min(s.MaxHp, s.Hp + amount);
        }
    }

    public override string ToString() => $"{Character?.Name}({Character?.Power})";
}
