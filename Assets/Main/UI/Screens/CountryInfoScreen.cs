using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CountryInfoScreen : IScreen
{
    public event EventHandler CloseButtonClicked;

    public void Initialize()
    {
        CharacterTable.Initialize();
        CharacterTable.RowMouseMove += CharacterTable_RowMouseMove; ;

        buttonClose.clicked += () => CloseButtonClicked?.Invoke(this, EventArgs.Empty);
    }

    private void CharacterTable_RowMouseMove(object sender, Character chara)
    {
        if (chara == characterInfoTarget) return;
        characterInfoTarget = chara;
        CharacterInfo.SetData(chara, country);
    }

    private Country country;
    private Area area;
    private Character characterInfoTarget;

    public void ShowCellInformation(WorldData world, MapPosition pos)
    {
        area = world.Map.GetArea(pos);
        country = world.CountryOf(area);
        var ruler = country.Ruler;
        characterInfoTarget = ruler;

        // 地形情報
        labelTerrain.text = TerrainName(area.Terrain);
        labelPosition.text = pos.ToString();

        // 勢力情報
        CountryRulerInfo.SetData(country, world);
        // 人物情報テーブル
        CharacterTable.SetData(country.Members, world);
        // 人物詳細
        CharacterInfo.SetData(ruler, country);
    }

    public static string TerrainName(Terrain terrain)
    {
        return terrain switch
        {
            Terrain.LargeRiver => "大河",
            Terrain.River => "河川",
            Terrain.Plain => "平地",
            Terrain.Hill => "丘陵",
            Terrain.Forest => "森林",
            Terrain.Mountain => "山岳",
            Terrain.Fort => "城砦",
            _ => throw new ArgumentOutOfRangeException(nameof(terrain)),
        };
    }
}