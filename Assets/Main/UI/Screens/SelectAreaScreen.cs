using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SelectAreaScreen : IScreen
{
    private AwaitableCompletionSource<Area> tcs;

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
            if (currentSelectedArea == null) return;
            tcs.SetResult(currentSelectedArea);
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
        currentSelectedArea = area;
        characterInfoTarget = null;

        var (canSelect, description) = canSelectArea(area);
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
    private Area currentSelectedArea;
    private Func<Area, (bool, string)> canSelectArea;

    public async Awaitable<Area> Show(
        string description,
        WorldData world,
        Func<Area, (bool, string)> canSelectArea)
    {
        tcs = new AwaitableCompletionSource<Area>();
        this.world = world;
        this.canSelectArea = canSelectArea;
        characterInfoTarget = null;
        buttonSelect.SetEnabled(false);

        labelDescription.text = description;

        GameCore.Instance.Tilemap.SetDisableIconForAreas(a => !canSelectArea(a).Item1);
        using var _ = GameCore.Instance.Tilemap.SetCellClickHandler(OnTileClick);

        // 勢力情報
        CountryRulerInfo.SetData(null, world);
        // 人物情報テーブル
        CharacterTable.SetData(null, world);
        // 人物詳細
        CharacterInfo.SetData(null, world);

        var selection = await tcs.Awaitable;

        GameCore.Instance.Tilemap.ResetDisableIcon();

        return selection;
    }
}