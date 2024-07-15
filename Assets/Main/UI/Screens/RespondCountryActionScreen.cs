using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public partial class RespondCountryActionScreen : IScreen
{
    private ValueTaskCompletionSource<bool> tcs;

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

        buttonYes.clicked += () =>
        {
            tcs.SetResult(true);
        };
        buttonNo.clicked += () =>
        {
            tcs.SetResult(false);
        };
    }

    private Character characterInfoTarget;
    private WorldData world;

    public ValueTask<bool> Show(
        string description,
        string yesText,
        string noText,
        Country country,
        WorldData world)
    {
        tcs = new ValueTaskCompletionSource<bool>();
        this.world = world;

        var ruler = country.Ruler;
        characterInfoTarget = ruler;

        labelDescription.text = description;
        buttonYes.text = yesText;
        buttonNo.text = noText;

        // 勢力情報
        CountryRulerInfo.SetData(country, world);
        // 人物情報テーブル
        CharacterTable.SetData(country.Members, world);
        // 人物詳細
        CharacterInfo.SetData(ruler, country);

        return tcs.Task;
    }
}