using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;
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
    public int Experience { get; set; }
    /// <summary>
    /// HP
    /// </summary>
    public int Hp { get; set; }

    public int MaxHp => Level * 10;

    public bool IsEmptySlot { get; set; }
    public bool IsAlive => !IsEmptySlot && Hp > 0;

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
    /// 地形
    /// </summary>
    public Terrain Terrain { get; set; }

    public MapPosition Position { get; set; }

    public Direction GetDirectionTo(Area target)
    {
        if (Position.x < target.Position.x) return Direction.Right;
        if (Position.x > target.Position.x) return Direction.Left;
        if (Position.y < target.Position.y) return Direction.Down;
        if (Position.y > target.Position.y) return Direction.Up;
        throw new InvalidOperationException();
    }

    public override string ToString() => $"Area {Position} ({Terrain})";
}

public struct MapPosition : IEquatable<MapPosition>
{
    public int x;
    public int y;

    public static MapPosition FromGrid(Vector3Int grid) => new() { x = grid.x, y = -grid.y };
    public static MapPosition Of(int x, int y) => new() { x = x, y = y };
    public readonly MapPosition Up => Of(x, y - 1);
    public readonly MapPosition Down => Of(x, y + 1);
    public readonly MapPosition Left => Of(x - 1, y);
    public readonly MapPosition Right => Of(x + 1, y);

    public readonly Vector3Int Vector3Int => new(x, -y, 0);

    public override readonly string ToString() => $"({x}, {y})";

    public readonly bool Equals(MapPosition other) => x == other.x && y == other.y;
    public static bool operator ==(MapPosition left, MapPosition right) => left.Equals(right);
    public static bool operator !=(MapPosition left, MapPosition right) => !(left == right);
    public override readonly bool Equals(object obj) => obj is MapPosition other && Equals(other);
    public override readonly int GetHashCode() => HashCode.Combine(x, y);
}

/// <summary>
/// 地形
/// </summary>
public enum Terrain
{
    LargeRiver,
    River,
    Plain,
    Hill,
    Forest,
    Mountain,
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

    /// <summary>
    /// マップの国の色のインデックス
    /// </summary>
    public int ColorIndex { get; set; }

    /// <summary>
    /// 雇える配下の最大数
    /// </summary>
    public int VassalCountMax => Math.Clamp((int)Math.Ceiling(Areas.Count / 2f), 2, 8);

    public IEnumerable<Character> Members => new[] { Ruler }.Concat(Vassals.ToArray());

    public override string ToString() => $"Country ID:{Id} ({Areas.Count} areas) {Ruler.Name}";
}

/// <summary>
/// マップ
/// </summary>
public class MapGrid
{
    public TilemapHelper Helper { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Area[] Areas { get; set; }

    public Area GetIndexOf(int index) => Areas[index];
    public Area GetArea(MapPosition p) => GetIndexOf(GetIndex(p));
    public int GetIndex(MapPosition p) => p.y * Width + p.x;
    public MapPosition GetXY(int index) => MapPosition.Of(index % Width, index / Width);

    public bool IsValid(MapPosition p) => p.x >= 0 && p.x < Width && p.y >= 0 && p.y < Height;

    public Area GetAbove(Area area) => IsValid(area.Position.Up) ? GetArea(area.Position.Up) : null;
    public Area GetBelow(Area area) => IsValid(area.Position.Down) ? GetArea(area.Position.Down) : null;
    public Area GetLeft(Area area) => IsValid(area.Position.Left) ? GetArea(area.Position.Left) : null;
    public Area GetRight(Area area) => IsValid(area.Position.Right) ? GetArea(area.Position.Right) : null;

    public IEnumerable<Area> GetNeighbors(Area area)
    {
        if (IsValid(area.Position.Up)) yield return GetAbove(area);
        if (IsValid(area.Position.Down)) yield return GetBelow(area);
        if (IsValid(area.Position.Left)) yield return GetLeft(area);
        if (IsValid(area.Position.Right)) yield return GetRight(area);
    }
}

public class DefaultData
{
    public static readonly List<string> NameList = new() { "アイリス", "アクエリアス", "アステル", "アストレア", "アトラス", "アネモネ", "アマンダ", "アリアドネ", "アリアナ", "アリエル", "アリシア", "アルタイル", "アルバート", "アーサー", "イカロス", "イグレイン", "イザベラ", "イゼリア", "イゼルト", "イゾルデ", "イリス", "イーリス", "ウィリアム", "エスメラルダ", "エドワード", "エマ", "エルシア", "エルロン", "エレノア", "オスカー", "オベロン", "オリオン", "オリビア", "オーロラ", "カトリーナ", "カリオペ", "ガイアス", "キャスパー", "グリフィン", "ケイルン", "ケイロス", "ケルベロス", "ザファーラ", "ザフィール", "ザンダー", "シエナ", "シエラ", "シャーロット", "シルバーン", "ジェームズ", "ジャスパー", "ジャスミン", "ジョージ", "ジークフリート", "セバスチャン", "セレスタ", "セレスティア", "セレーネ", "ゼノン", "ゼファー", "ゼラン", "ソフィア", "ソレイユ", "タリシア", "チャールズ", "デューン", "トリスタン", "ドラゴミール", "ドリアン", "ナイア", "ナオミ", "ナディア", "ネメア", "ネロ", "ノクターン", "ハイペリオン", "ビアンカ", "ビクトリア", "ファエリン", "フェニックス", "フリージア", "フレデリック", "プロメテウス", "ヘリオス", "ヘンリー", "ペルシヴァル", "マーガレット", "ミリアム", "ライオネル", "ライラ", "ラベンダー", "リリス", "ルシアン", "ルナ", "レイモンド", "レイヴン", "レオナルド", "ロザリンド", "ロゼッタ", "ローエン", "ローズマリー", "ヴァルカン", "ヴィオレット", };
    public static Character[] GetDefaultCharacterList()
    {
        var rand = new System.Random(0);
        return NameList.Select((name, i) =>
        {
            var luck = rand.Next(0, 10) * 2;
            return new Character()
            {
                Id = i,
                Name = name,
                Attack = Math.Min(Dice(10, 10) + Random.Range(-20, 20) + luck, 100),
                Defense = Math.Min(Dice(10, 10) + Random.Range(-20, 20) + luck, 100),
                Intelligence = Math.Min(Dice(10, 10) + Random.Range(-20, 20) + luck, 100),
                LoyaltyBase = Math.Min(Dice(10, 10) + Random.Range(-20, 20), 100),
            };
        }).ToArray();
    }
    private static int Dice(int sides) => Random.Range(1, sides + 1);
    private static int Dice(int sides, int count) => Enumerable.Range(0, count).Sum(_ => Dice(sides));


    public static MapGrid CreateMapGrid(int size, TilemapHelper helper)
    {
        var map = new MapGrid();
        map.Helper = helper;
        map.Width = size;
        map.Height = size;
        map.Areas = new Area[size * size];
        var nextId = 1;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                var area = new Area();
                area.Id = nextId++;
                area.Position = MapPosition.Of(x, y);
                area.Terrain = helper.GetTerrain(area.Position);
                map.Areas[y * size + x] = area;
            }
        }
        return map;
    }

    public static Force DefaultForce()
    {
        return new Force()
        {
            Soldiers = Enumerable.Range(0, 10)
                .Select(i => new Soldier() { Level = 1, IsEmptySlot = false })
                .ToArray(),
        };
    }

    public static WorldData InitializeDefaultData(TilemapData tilemapData, TilemapHelper helper)
    {
        var rand = new System.Random((int)DateTime.Now.Ticks);
        var characters = GetDefaultCharacterList();
        var map = CreateMapGrid(TilemapData.Width, helper);
        var charas = new List<Character>(characters);

        var countryIdToColorId = new HashSet<int>(tilemapData.countryTileIndex).ToArray();

        var countries = new List<Country>();
        for (int iCountry = 0; iCountry < countryIdToColorId.Length; iCountry++)
        {
            // 適当なキャラを君主として選ぶ。
            var ruler = charas[rand.Next(0, charas.Count)];
            ruler.Gold = 100;
            ruler.Prestige = 10;
            ruler.Force = DefaultForce();
            charas.Remove(ruler);

            var country = new Country
            {
                Id = iCountry,
                Ruler = ruler,
                Vassals = new List<Character>(),
                Areas = new List<Area>(),
                ColorIndex = countryIdToColorId[iCountry],
            };
            countries.Add(country);

            // エリアを割り当てる。
            for (int iArea = 0; iArea < map.Areas.Length; iArea++)
            {
                if (tilemapData.countryTileIndex[iArea] == country.ColorIndex)
                {
                    var area = map.GetIndexOf(iArea);
                    country.Areas.Add(area);
                }
            }

            // 適当なキャラを配下に割り当てる。
            var vassalCount = country.VassalCountMax - 1;
            for (int iChar = 0; iChar < vassalCount; iChar++)
            {
                var vassal = charas[rand.Next(0, charas.Count)];
                vassal.Gold = 10 * (1 + iChar);
                vassal.Prestige = (1 + iChar) * 2;
                vassal.SalaryRatio = 10 + 5 * iChar;
                vassal.Force = DefaultForce();
                charas.Remove(vassal);
                country.Vassals.Add(vassal);
            }
            ruler.SalaryRatio = 100 - country.Vassals.Sum(v => v.SalaryRatio);
        }

        // 未所属のキャラを初期化する。
        foreach (var chara in charas)
        {
            chara.Gold = 10;
            chara.Prestige = 0;
            chara.Force = DefaultForce();
        }
        
        return new WorldData()
        {
            Characters = characters,
            Countries = countries,
            Map = map,
        };
    }
}
