using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SelectCountryScreen : IScreen
{
    private ValueTaskCompletionSource<Country> tcs;

    public LocalizationManager L => MainUI.Instance.L;
    public void Initialize()
    {
        CharacterTable.Initialize();

        // マウスオーバー時
        CharacterTable.RowMouseMove += (sender, chara) =>
        {
            if (chara == characterInfoTarget) return;
            characterInfoTarget = chara;
            CharacterInfo.SetData(chara, world);
        };

        buttonSelect.clicked += () =>
        {
            if (currentSelectedCountry == null) return;
            tcs.SetResult(currentSelectedCountry);
        };

        // キャンセルされた場合
        buttonClose.clicked += () =>
        {
            tcs.SetResult(null);
        };
    }

    private void OnTileClick(object sender, MapPosition pos)
    {
        var area = world.Map.GetArea(pos);
        var country = world.CountryOf(area);
        currentSelectedCountry = country;
        characterInfoTarget = null;

        var (canSelect, description) = canSelectCountry(country);
        labelDescription.text = description;
        buttonSelect.SetEnabled(canSelect);

        // 勢力情報
        CountryRulerInfo.SetData(country, world);
        // 人物情報テーブル
        CharacterTable.SetData(country.Members, world);
        // 人物詳細
        CharacterInfo.SetData(null, world);
    }

    private Character characterInfoTarget;
    private WorldData world;
    private Country currentSelectedCountry;
    private Func<Country, (bool, string)> canSelectCountry;

    public async ValueTask<Country> Show(
        string description,
        WorldData world,
        Func<Country, (bool, string)> canSelectCountry)
    {
        tcs = new ValueTaskCompletionSource<Country>();
        this.world = world;
        this.canSelectCountry = canSelectCountry;
        characterInfoTarget = null;
        buttonSelect.SetEnabled(false);

        labelDescription.text = description;

        GameCore.Instance.Tilemap.SetDisableIcon(c => !canSelectCountry(c).Item1);
        using var _ = GameCore.Instance.Tilemap.SetCellClickHandler(OnTileClick);

        // 勢力情報
        CountryRulerInfo.SetData(null, world);
        // 人物情報テーブル
        CharacterTable.SetData(null, world);
        // 人物詳細
        CharacterInfo.SetData(null, world);

        var selection = await tcs.Task;

        GameCore.Instance.Tilemap.ResetDisableIcon();

        return selection;
    }
}