using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
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

    /// <summary>
    /// プレーヤーならtrue
    /// </summary>
    public bool IsPlayer { get; set; }
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

    public int MaxHp => Level * 10;

    public bool IsEmptySlot { get; set; }
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

    public CharacterManager CreateCharacters()
    {


        return new CharacterManager();
    }
}

public class CharacterManager
{
    public Character[] Characters { get; set; }
}

public class CountryManager
{
    public List<Country> Countries { get; set; }
}


public static class Util
{
    public static TEnum[] EnumArray<TEnum>()
    {
        return (TEnum[])Enum.GetValues(typeof(TEnum));
    }
}




public class DefaultData
{
    public static readonly List<string> NameList = new() { "アイリス", "アクエリアス", "アステル", "アストレア", "アトラス", "アネモネ", "アマンダ", "アリアドネ", "アリアナ", "アリエル", "アリシア", "アルタイル", "アルバート", "アーサー", "イカロス", "イグレイン" "イザベラ", "イゼリア", "イゼルト", "イゾルデ", "イリス", "イーリス", "ウィリアム", "エスメラルダ", "エドワード", "エマ", "エルシア", "エルロン", "エレノア", "オスカー", "オベロン", "オリオン", "オリビア", "オーロラ", "カトリーナ", "カリオペ", "ガイアス", "キャスパー", "グリフィン", "ケイルン", "ケイロス", "ケルベロス", "ザファーラ", "ザフィール", "ザンダー", "シエナ", "シエラ", "シャーロット", "シルバーン", "ジェームズ", "ジャスパー", "ジャスミン", "ジョージ", "ジークフリート" "セバスチャン", "セレスタ", "セレスティア", "セレーネ", "ゼノン", "ゼファー", "ゼラン", "ソフィア", "ソレイユ", "タリシア", "チャールズ", "デューン", "トリスタン", "ドラゴミール", "ドリアン", "ナイア", "ナオミ", "ナディア", "ネメア", "ネロ", "ノクターン", "ハイペリオン", "ビアンカ", "ビクトリア", "ファエリン", "フェニックス", "フリージア", "フレデリック", "プロメテウス", "ヘリオス", "ヘンリー", "ペルシヴァル", "マーガレット", "ミリアム", "ライオネル", "ライラ", "ラベンダー", "リリス", "ルシアン", "ルナ", "レイモンド", "レイヴン", "レオナルド", "ロザリンド", "ロゼッタ", "ローエン", "ローズマリー", "ヴァルカン", "ヴィオレット", };
    public static Character[] GetDefaultCharacterList()
    {
        var rand = new System.Random(0);
        return NameList.Select((name, i) =>
        {
            return new Character()
            {
                Id = i,
                Name = name,
                Attack = rand.Next(1, 100),
                Defense = rand.Next(1, 100),
                Intelligence = rand.Next(1, 100),
            };
        }).ToArray();
    }

    public static MapGrid CreateMapGrid(int size)
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

    public static WorldData InitializeDefaultData()
    {
        var rand = new System.Random(0);
        var countryCount = 9;
        var vassalCount = 3;
        var characters = GetDefaultCharacterList();
        var map = CreateMapGrid(9);
        var charas = new List<Character>(characters);

        var countries = new List<Country>();
        for (int iCountry = 0; iCountry < countryCount; iCountry++)
        {
            // 適当なキャラを君主として選ぶ。
            var ruler = charas[rand.Next(0, charas.Count)];
            ruler.Gold = 100;
            ruler.Prestige = 10;
            charas.Remove(ruler);

            var country = new Country
            {
                Id = iCountry,
                Ruler = ruler,
                Vassals = new List<Character>(),
                Areas = new List<Area>(),
            };
            countries.Add(country);
            // 適当なキャラを配下に割り当てる。
            for (int iChar = 0; iChar < vassalCount; iChar++)
            {
                var vassal = charas[rand.Next(0, charas.Count)];
                vassal.Gold = 10 * (1 + iChar);
                vassal.Prestige = iChar * 2;
                vassal.SalaryRatio = 10 * iChar;
                charas.Remove(vassal);
                country.Vassals.Add(vassal);
            }
            ruler.SalaryRatio = 100 - country.Vassals.Sum(v => v.SalaryRatio);

            // 適当なエリアを割り当てる。
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    var area = map.Areas[y + 3 * (iCountry / 3), x + 3 * (iCountry % 3)];
                    country.Areas.Add(area);
                }
            }
        }
        
        return new WorldData()
        {
            Characters = characters,
            Countries = countries,
            Map = map,
        };
    }
}

public class WorldData
{
    public Character[] Characters { get; set; }
    public List<Country> Countries { get; set; }
    public MapGrid Map { get; set; }

    public bool IsRuler(Character chara) => Countries.Any(c => c.Ruler == chara);
    public bool IsVassal(Character chara) => Countries.Any(c => c.Vassals.Contains(chara));
    public bool IsFree(Character chara) => !IsRuler(chara) && !IsVassal(chara);
}
