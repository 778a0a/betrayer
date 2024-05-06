using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class OrganizeScreen : IScreen
{
    private AwaitableCompletionSource<bool> tcs;

    public void Initialize()
    {
        CharacterTable.Initialize();

        // マウスオーバー時
        CharacterTable.RowMouseMove += (sender, chara) =>
        {
        };

        // 選択された場合
        CharacterTable.RowMouseDown += (sender, chara) =>
        {
            if (chara == characterTarget) return;

            characterTarget = chara;
            Refresh();
        };

        // 増やす
        buttonIncrease.clicked += () =>
        {
            if (characterTarget == null) return;
            if (world.IsRuler(characterTarget)) return;
            if (country.Ruler.SalaryRatio <= Character.SalaryRatioMin) return;
            if (characterTarget.SalaryRatio >= Character.SalaryRatioMax) return;
            characterTarget.SalaryRatio++;
            country.RecalculateSalary();
            Refresh();
        };

        // 減らす
        buttonDecrease.clicked += () =>
        {
            if (characterTarget == null) return;
            if (world.IsRuler(characterTarget)) return;
            if (country.Ruler.SalaryRatio >= Character.SalaryRatioMax) return;
            if (characterTarget.SalaryRatio <= Character.SalaryRatioMin) return;
            characterTarget.SalaryRatio--;
            country.RecalculateSalary();
            Refresh();
        };

        // 上へ
        buttonUp.clicked += () =>
        {
            if (characterTarget == null) return;
            if (world.IsRuler(characterTarget)) return;
            var index = country.Vassals.IndexOf(characterTarget);
            if (index == 0) return;
            country.Vassals.RemoveAt(index);
            country.Vassals.Insert(index - 1, characterTarget);
            country.RecalculateSalary();
            Refresh();
        };

        // 下へ
        buttonDown.clicked += () =>
        {
            if (characterTarget == null) return;
            if (world.IsRuler(characterTarget)) return;
            var index = country.Vassals.IndexOf(characterTarget);
            if (index == country.Vassals.Count - 1) return;
            country.Vassals.RemoveAt(index);
            country.Vassals.Insert(index + 1, characterTarget);
            country.RecalculateSalary();
            Refresh();
        };

        // 完了
        buttonClose.clicked += () =>
        {
            tcs.SetResult(true);
        };
    }

    private Character characterTarget;
    private WorldData world;
    private Country country;

    public Awaitable<bool> Show(
        Country country,
        WorldData world)
    {
        tcs = new AwaitableCompletionSource<bool>();
        this.world = world;
        this.country = country;
        characterTarget = null;

        Refresh();

        return tcs.Awaitable;
    }

    private void Refresh()
    {
        // 人物情報テーブル
        CharacterTable.SetData(country.Members, world);

        // 人物詳細
        CharacterInfo.SetData(characterTarget, world);

        labelTargetLoyalty.text = "--";
        labelTargetSalaryRatio.text = "--";
        if (characterTarget != null)
        {
            labelTargetLoyalty.text = characterTarget.Loyalty.ToString();
            labelTargetSalaryRatio.text = characterTarget.SalaryRatio.ToString();
        }

        var isRuler = world.IsRuler(characterTarget);
        buttonIncrease.SetEnabled(!isRuler);
        buttonDecrease.SetEnabled(!isRuler);
        buttonUp.SetEnabled(!isRuler);
        buttonDown.SetEnabled(!isRuler);
    }
}