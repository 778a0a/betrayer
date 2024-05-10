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

        // �}�E�X�I�[�o�[��
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

        // �L�����Z�����ꂽ�ꍇ
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

        // ���͏��
        CountryRulerInfo.SetData(country, world);
        // �l�����e�[�u��
        CharacterTable.SetData(country.Members, world);
        // �l���ڍ�
        CharacterInfo.SetData(null, world);
    }

    private Character characterInfoTarget;
    private WorldData world;
    private Area currentSelectedArea;
    private Func<Area, (bool, string)> canSelectArea;

    public Awaitable<Area> Show(
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

        Test.Instance.tilemap.TileClick += OnTileClick;
        tcs.Awaitable.GetAwaiter().OnCompleted(() =>
        {
            Test.Instance.tilemap.TileClick -= OnTileClick;
        });

        // ���͏��
        CountryRulerInfo.SetData(null, world);
        // �l�����e�[�u��
        CharacterTable.SetData(null, world);
        // �l���ڍ�
        CharacterInfo.SetData(null, world);

        return tcs.Awaitable;
    }
}