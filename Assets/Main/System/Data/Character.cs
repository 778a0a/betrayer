using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// キャラクター
/// </summary>
public class Character
{
    public static int SalaryRatioMin = 0;
    public static int SalaryRatioMax = 100;

    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 顔画像インデックス
    /// </summary>
    public int ImageIndex { get; set; }
    /// <summary>
    /// 名前
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 攻撃
    /// </summary>
    public int Attack { get; set; }
    /// <summary>
    /// 防御
    /// </summary>
    public int Defense { get; set; }
    /// <summary>
    /// 智謀
    /// </summary>
    public int Intelligence { get; set; }
    /// <summary>
    /// 忠誠基本値
    /// </summary>
    public int LoyaltyBase { get; set; }

    /// <summary>
    /// 所持金
    /// </summary>
    public int Gold { get; set; }
    /// <summary>
    /// 功績
    /// </summary>
    public int Contribution { get; set; }
    /// <summary>
    /// 名声
    /// </summary>
    public int Prestige { get; set; }
    /// <summary>
    /// 忠誠
    /// </summary>
    public int Loyalty { get; set; }
    /// <summary>
    /// 給料配分
    /// </summary>
    public int SalaryRatio { get; set; }

    /// <summary>
    /// 軍勢
    /// </summary>
    public Force Force { get; set; }

    /// <summary>
    /// プレーヤーならtrue
    /// </summary>
    public bool IsPlayer { get; set; }

    /// <summary>
    /// 侵攻済みならtrue
    /// </summary>
    public bool IsAttacked { get; set; }

    /// <summary>
    /// （内部データ）強さ
    /// </summary>
    public int Power => (Attack + Defense + Intelligence) / 3 * Force.Power;

    public string debugImagePath { get; set; }
    public string debugMemo { get; set; }

    /// <summary>
    /// 恨み
    /// </summary>
    public int Urami { get; set; } = 0;

    /// <summary>
    /// 地位
    /// </summary>
    public string GetTitle(WorldData world)
    {
        if (world.IsRuler(this))
        {
            var country = world.CountryOf(this);
            return country.CountryRank switch
            {
                CountryRank.Empire => "皇帝",
                CountryRank.Kingdom => "王",
                CountryRank.Duchy => "大公",
                _ => "君主",
            };
        }
        else if (world.IsVassal(this))
        {
            var country = world.CountryOf(this);
            var order = country.VassalCountMax - country.Vassals.IndexOf(this) - 1;
            return new[]
            {
                "従士",
                "従士",
                "士長",
                "将軍",
                "元帥",
                "宰相",
                "総督",
                "副王",
            }[order];
        }
        else
        {
            return "浪士";
        }
    }

    public override string ToString() => $"{Name} G:{Gold} P:{Power} (A:{Attack} D:{Defense} I:{Intelligence})";
}

/// <summary>
/// 兵士
/// </summary>
public class Soldier
{
    /// <summary>
    /// レベル
    /// </summary>
    public int Level { get; set; }
    /// <summary>
    /// 経験値
    /// </summary>
    public float Experience { get; set; }
    /// <summary>
    /// HP
    /// </summary>
    public int Hp
    {
        get => (int)Mathf.Ceil(HpFloat);
        set => HpFloat = value;
    }

    public float HpFloat { get; set; }

    public int MaxHp => Level * 5 + 30;

    public bool IsEmptySlot { get; set; }
    public bool IsAlive => !IsEmptySlot && HpFloat > 0;

    public override string ToString() => IsEmptySlot ? "Empty" : $"Lv{Level} HP{Hp}/{MaxHp} Exp:{Experience}";
    public string ToShortString() => IsEmptySlot ? "E" : $"{Level}";
}

/// <summary>
/// 軍勢
/// </summary>
public class Force
{
    /// <summary>
    /// 兵士
    /// </summary>
    public Soldier[] Soldiers { get; set; }

    public bool HasEmptySlot => Soldiers.Any(s => s.IsEmptySlot);

    public int Power => Soldiers.Sum(s => s.IsEmptySlot ? 0 : s.Level);

    public override string ToString() => $"Power:{Power} ({string.Join(",", Soldiers.Select(s => s.ToShortString()))})";
}