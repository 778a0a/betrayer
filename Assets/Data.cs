using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// キャラクター
/// </summary>
public class Character
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }
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
    public int Experience { get; set; }
    /// <summary>
    /// HP
    /// </summary>
    public int Hp { get; set; }
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
}

/// <summary>
/// 地域
/// </summary>
public class Area
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 攻撃側の地形
    /// </summary>
    public Terrain AttackSideTerrain { get; set; }
    /// <summary>
    /// 防御側の地形
    /// </summary>
    public Terrain DefenseSideTerrain { get; set; }
}

/// <summary>
/// 地形
/// </summary>
public enum Terrain
{
    Swamp,
    River,
    Wasteland,
    Plain,
    Forest,
    Fort,
}

/// <summary>
/// 国
/// </summary>
public class Country
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 君主
    /// </summary>
    public Character Ruler { get; set; }
    /// <summary>
    /// 配下
    /// </summary>
    public List<Character> Vassals { get; set; }
    /// <summary>
    /// 領地
    /// </summary>
    public List<Area> Areas { get; set; }
    /// <summary>
    /// 同盟国
    /// </summary>
    public Country Ally { get; set; }
}

/// <summary>
/// マップ
/// </summary>
public class MapGrid
{
    public Area[,] Areas { get; set; }
}

public class Initializer
{
    /// <summary>
    /// マップを初期化します。
    /// </summary>
    public MapGrid CreateMapGrid(int size)
    {
        var map = new MapGrid();
        map.Areas = new Area[size, size];
        var terrains = Util.EnumArray<Terrain>();
        var nextId = 1;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                var area = new Area();
                area.Id = nextId++;
                area.AttackSideTerrain = terrains[Random.Range(0, terrains.Length)];
                area.DefenseSideTerrain = terrains[Random.Range(0, terrains.Length)];
                map.Areas[y, x] = area;

            }
        }
        return map;
    }

    public CharacterManager CreateCharacters()
    {


        return new CharacterManager();
    }
}

public class CharacterManager
{
    public Character[] Characters { get; set; }
}

public static class Util
{
    public static TEnum[] EnumArray<TEnum>()
    {
        return (TEnum[])Enum.GetValues(typeof(TEnum));
    }
}