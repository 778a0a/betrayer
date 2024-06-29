using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SelectPlayerCaharacterScreen : IScreen
{
    public event EventHandler<Character> CharacterSelected;

    public void Initialize()
    {
        CharacterTable.Initialize();
        CharacterTable.RowMouseMove += CharacterTable_RowMouseMove;

        CharacterTable.RowMouseDown += (sender, chara) =>
        {
            if (chara == null) return;
            OnCharacterSelected(chara);
        };

        buttonRandom.clicked += () =>
        {
            var world = GameCore.Instance.World;
            var chara = world.Characters.RandomPick();
            OnCharacterSelected(chara);
        };

        buttonShowFreeList.clicked += () =>
        {
        };

        async void OnCharacterSelected(Character chara)
        {
            var world = GameCore.Instance.World;
            var res = await MessageWindow.Show(
                $"「{chara.GetTitle(world)} {chara.Name}」でゲームを始めます。\nよろしいですか？",
                MessageBoxButton.OkCancel);
            if (res != MessageBoxResult.Ok) return;

            CharacterSelected?.Invoke(this, chara);
        }
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

    public void ShowCellInformation(WorldData world, MapPosition? pos)
    {
        if (pos == null)
        {
            labelDescription.text = "マップのセルをクリックしてください。";
            CountryRulerInfo.Root.style.display = DisplayStyle.None;
            CharacterTable.Root.style.display = DisplayStyle.None;
            CharacterInfo.Root.style.display = DisplayStyle.None;
            return;
        }
        labelDescription.text = "一覧表から操作するキャラをクリックしてください。";
        CountryRulerInfo.Root.style.display = DisplayStyle.Flex;
        CharacterTable.Root.style.display = DisplayStyle.Flex;
        CharacterInfo.Root.style.display = DisplayStyle.Flex;

        area = world.Map.GetArea(pos.Value);
        country = world.CountryOf(area);
        var ruler = country.Ruler;
        characterInfoTarget = ruler;

        //// 地形情報
        //labelTerrain.text = CountryInfoScreen.TerrainName(area.Terrain);
        //labelPosition.text = pos.ToString();

        // 勢力情報
        CountryRulerInfo.SetData(country, world);
        // 人物情報テーブル
        CharacterTable.SetData(country.Members, world);
        // 人物詳細
        CharacterInfo.SetData(ruler, country);
    }
}