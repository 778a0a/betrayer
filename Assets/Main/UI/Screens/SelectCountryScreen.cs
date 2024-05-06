using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SelectCountryScreen : IScreen
{
    private AwaitableCompletionSource<Country> tcs;

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

        if (country.Ally != null)
        {
            labelDescription.text = $"すでに別の国と同盟を結んでいます。";
            buttonSelect.SetEnabled(false);
        }
        else
        {
            labelDescription.text = $"この国と同盟を結びますか？";
            buttonSelect.SetEnabled(true);
        }

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

    public Awaitable<Country> Show(
        string description,
        WorldData world)
    {
        tcs = new AwaitableCompletionSource<Country>();
        this.world = world;
        characterInfoTarget = null;
        buttonSelect.SetEnabled(false);

        labelDescription.text = description;

        Test.Instance.tilemap.TileClick += OnTileClick;
        tcs.Awaitable.GetAwaiter().OnCompleted(() =>
        {
            Test.Instance.tilemap.TileClick -= OnTileClick;
        });

        // 勢力情報
        CountryRulerInfo.SetData(null, world);
        // 人物情報テーブル
        CharacterTable.SetData(null, world);
        // 人物詳細
        CharacterInfo.SetData(null, world);

        return tcs.Awaitable;
    }
}