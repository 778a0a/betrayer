using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SelectCharacterScreen : IScreen
{
    private AwaitableCompletionSource<Character> tcs;

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

    public Awaitable<Character> Show(
        string description,
        string cancelText,
        IList<Character> charas,
        WorldData world, 
        Predicate<Character> predCanSelect = null)
    {
        tcs = new AwaitableCompletionSource<Character>();
        this.world = world;
        this.predCanSelect = predCanSelect;

        labelDescription.text = description;
        buttonClose.text = cancelText;

        // 人物情報テーブル
        CharacterTable.SetData(charas, world);
        
        // 人物詳細
        CharacterInfo.SetData(charas[0], world);

        return tcs.Awaitable;
    }


}