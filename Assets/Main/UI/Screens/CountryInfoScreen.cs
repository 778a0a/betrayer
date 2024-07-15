using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CountryInfoScreen : IScreen
{
    public event EventHandler CloseButtonClicked;

    public LocalizationManager L => GameCore.Instance.MainUI.L;
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
        var L = GameCore.Instance.MainUI.L;
        return terrain switch
        {
            Terrain.LargeRiver => L["大河"],
            Terrain.River => L["河川"],
            Terrain.Plain => L["平地"],
            Terrain.Hill => L["丘陵"],
            Terrain.Forest => L["森林"],
            Terrain.Mountain => L["山岳"],
            Terrain.Fort => L["城砦"],
            _ => throw new ArgumentOutOfRangeException(nameof(terrain)),
        };
    }
}