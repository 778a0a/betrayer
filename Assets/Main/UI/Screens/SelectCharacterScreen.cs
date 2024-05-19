using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SelectCharacterScreen : IScreen
{
    private ValueTaskCompletionSource<Character> tcs;

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

        // 選択された場合
        CharacterTable.RowMouseDown += (sender, chara) =>
        {
            if (!(predCanSelect?.Invoke(chara) ?? true)) return;
            tcs.SetResult(chara);
        };

        // キャンセルされた場合
        buttonClose.clicked += () =>
        {
            tcs.SetResult(null);
        };
    }

    private Character characterInfoTarget;
    private WorldData world;
    private Predicate<Character> predCanSelect;

    public async ValueTask<Character> Show(
        string description,
        string cancelText,
        IList<Character> charas,
        WorldData world, 
        Predicate<Character> predCanSelect = null)
    {
        tcs = new ValueTaskCompletionSource<Character>();
        this.world = world;
        this.predCanSelect = predCanSelect;

        labelDescription.text = description;
        buttonClose.text = cancelText;

        // 人物情報テーブル
        CharacterTable.SetData(charas, world);
        
        // 人物詳細
        CharacterInfo.SetData(charas[0], world);

        return await tcs.Task;
    }


    public async ValueTask<Character> ShowForProvoke(
        string description,
        WorldData world,
        Func<Area, (bool, string)> canSelectArea)
    {
        tcs = new ValueTaskCompletionSource<Character>();
        this.world = world;
        predCanSelect = null;
        characterInfoTarget = null;

        labelDescription.text = description;

        void OnTileClick(object sender, MapPosition pos)
        {
            var area = world.Map.GetArea(pos);
            var country = world.CountryOf(area);
            characterInfoTarget = null;

            var (canSelect, description) = canSelectArea(area);
            labelDescription.text = description;

            if (canSelect)
            {
                CharacterTable.SetData(country.Members, world);
                CharacterInfo.SetData(null, world);
            }
            else
            {
                CharacterTable.SetData(null, world);
                CharacterInfo.SetData(null, world);
            }

        }

        using var _ = GameCore.Instance.Tilemap.SetCellClickHandler(OnTileClick);
        GameCore.Instance.Tilemap.SetDisableIconForAreas(a => !canSelectArea(a).Item1);

        // 人物情報テーブル
        CharacterTable.SetData(null, world);
        // 人物詳細
        CharacterInfo.SetData(null, world);

        var selection = await tcs.Task;

        GameCore.Instance.Tilemap.ResetDisableIcon();

        return selection;
    }

}