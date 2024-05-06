using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;


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
