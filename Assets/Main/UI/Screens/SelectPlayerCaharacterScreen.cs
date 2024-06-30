using System;
using System.Linq;
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

        buttonWatch.clicked += () =>
        {
            OnCharacterSelected(null);
        };

        buttonShowFreeList.clicked += () =>
        {
            isShowingFreeList = !isShowingFreeList;
            ShowCellInformation(GameCore.Instance.World, null);
        };

        buttonPrevPage.clicked += () =>
        {
            freeListPage = Mathf.Max(0, freeListPage - 1);
            ShowCellInformation(GameCore.Instance.World, null);
        };

        buttonNextPage.clicked += () =>
        {
            var world = GameCore.Instance.World;
            var frees = world.Characters.Where(c => world.IsFree(c)).ToList();
            var pageCount = 1 + frees.Count / 10;
            freeListPage = Mathf.Min(freeListPage + 1, pageCount - 1);
            ShowCellInformation(GameCore.Instance.World, null);
        };


        async void OnCharacterSelected(Character chara)
        {
            var world = GameCore.Instance.World;
            var res = await MessageWindow.Show(chara != null ?
                $"「{chara.GetTitle(world)} {chara.Name}」でゲームを始めます。\nよろしいですか？" :
                "観戦モードでゲームを始めます。\nよろしいですか？",
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

    private bool isShowingFreeList = false;
    private int freeListPage = 0;

    public void ShowCellInformation(WorldData world, MapPosition? pos)
    {
        // デバッグ中なら観戦を有効にする。
#if UNITY_EDITOR
        buttonWatch.style.display = DisplayStyle.Flex;
#else
        buttonWatch.style.display = DisplayStyle.None;
#endif

        if (pos == null && !isShowingFreeList)
        {
            labelDescription.text = "マップのセルをクリックしてください。";
            FreeListController.style.display = DisplayStyle.None;
            CountryRulerInfo.Root.style.display = DisplayStyle.None;
            CharacterTable.Root.style.display = DisplayStyle.None;
            CharacterInfo.Root.style.display = DisplayStyle.None;
            return;
        }
        if (pos == null && isShowingFreeList)
        {
            labelDescription.text = "一覧表から操作するキャラをクリックしてください。";
            FreeListController.style.display = DisplayStyle.Flex;
            CountryRulerInfo.Root.style.display = DisplayStyle.None;
            CharacterTable.Root.style.display = DisplayStyle.Flex;
            CharacterInfo.Root.style.display = DisplayStyle.Flex;

            var frees = world.Characters.Where(c => world.IsFree(c)).ToList();
            var pageCount = 1 + frees.Count / 10;
            var page = freeListPage;
            var charas = frees.Skip(page * 10).Take(10).ToList();
            CharacterTable.SetData(charas, world);
            characterInfoTarget = charas.FirstOrDefault();
            CharacterInfo.SetData(characterInfoTarget, country);
            labelPageNo.text = $"{page + 1}/{pageCount}";
            return;
        }
        isShowingFreeList = false;

        labelDescription.text = "一覧表から操作するキャラをクリックしてください。";
        FreeListController.style.display = DisplayStyle.None;
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