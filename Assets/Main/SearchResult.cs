using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SearchResult
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

    public Awaitable<Character> Show(Character[] charas, WorldData world)
    {
        tcs = new AwaitableCompletionSource<Character>();
        this.world = world;

        // 人物情報テーブル
        CharacterTable.SetData(charas, world);
        
        // 人物詳細
        CharacterInfo.SetData(charas[0], world);

        return tcs.Awaitable;
    }


}