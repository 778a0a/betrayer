using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

public record CharacterInBattle(
    Character Character,
    Terrain Terrain,
    Country Country,
    bool IsAttacker)
{
    public CharacterInBattle Opponent { get; set; }
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

    public bool ShouldRetreat()
    {
        // プレーヤーの場合はUIで判断しているので処理不要。
        if (IsPlayer) return false;

        // まだ損耗が多くないなら撤退しない。
        var manyLoss = Character.Force.Soldiers.Count(s => s.Hp < 10) >= 3;
        if (!manyLoss) return false;

        // 敵よりも兵力が多いなら撤退しない。
        var myPower = Force.Power;
        var opPower = Opponent.Force.Power;
        if (myPower > opPower) return false;

        // 敵に残り数の少ない兵士がいるなら撤退しない。
        var opAboutToDie = Opponent.Force.Soldiers.Any(s => s.Hp <= 3);
        if (opAboutToDie) return false;

        // 自国の最後の領土なら撤退しない。
        var lastArea = Country.Areas.Count == 1;
        if (lastArea) return false;

        // 撤退する。
        return true;
    }
}
