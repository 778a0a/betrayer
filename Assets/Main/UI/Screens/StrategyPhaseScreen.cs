using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class StrategyPhaseScreen : IScreen
{
    public event EventHandler<ActionButton> ActionButtonClicked;

    public enum ActionButton
    {
        ShowInfo,
        Organize,
        HireVassal,
        FireVassal,
        Ally,
        Resign,
        ShowSystemMenu,
        EndTurn,
    }

    public void Initialize()
    {
        var buttons = new[]
        {
            buttonShowInfo,
            buttonOrganize,
            buttonHireVassal,
            buttonFireVassal,
            buttonAlly,
            buttonResign,
            buttonShowSystemMenu,
            buttonEndTurn,
        };
        foreach (var button in buttons)
        {
            button.RegisterCallback<ClickEvent>(OnActionButtonClicked);
            button.RegisterCallback<PointerEnterEvent>(OnActionButtonPointerEnter);
            button.RegisterCallback<PointerLeaveEvent>(OnActionButtonPointerLeave);
        }
    }


    private void OnActionButtonPointerEnter(PointerEnterEvent evt)
    {
    }

    private void OnActionButtonPointerLeave(PointerLeaveEvent evt)
    {
    }

    private void OnActionButtonClicked(ClickEvent ev)
    {
        var button = (Button)ev.target;
        var actionName = button.name.Substring("button".Length);
        var action = Enum.Parse<ActionButton>(actionName);
        ActionButtonClicked?.Invoke(this, action);
    }

    private SoldierInfoLarge[] soldiers;
    private WorldData world;
    public Character debugCurrentChara;
    public void Refresh() => SetData(debugCurrentChara, world);
    public void SetData(Character chara, WorldData world)
    {
        this.world = world;
        debugCurrentChara = chara;
        if (chara == null && world == null) return;
        imageChara.image = FaceImageManager.Instance.GetImage(chara);
        labelName.text = chara.Name;
        labelTitle.text = chara.GetTitle(world);
        labelYearsOfService.text = "88";
        labelSalaryRatio.text = chara.SalaryRatio.ToString();

        labelCharaAttack.text = chara.Attack.ToString();
        labelCharaDefense.text = chara.Defense.ToString();
        labelCharaIntelligence.text = chara.Intelligence.ToString();
        labelCharaLoyalty.text = chara.Loyalty.ToString();
        labelCharaPrestige.text = chara.Prestige.ToString();
        labelCharaContribution.text = chara.Contribution.ToString();

        if (soldiers == null)
        {
            soldiers = new[] { soldier00, soldier01, soldier02, soldier03, soldier04, soldier05, soldier06, soldier07, soldier08, soldier09, soldier10, soldier11, soldier12, soldier13, soldier14, };
        }

        for (int i = 0; i < soldiers.Length; i++)
        {
            if (chara.Force.Soldiers.Length <= i)
            {
                soldiers[i].Root.style.visibility = Visibility.Hidden;
                continue;
            }

            soldiers[i].SetData(chara.Force.Soldiers[i]);
        }

        labelCost.text = "TODO";
        labelGold.text = chara.Gold.ToString();

        var country = world.CountryOf(chara);
        if (country != null)
        {
            CountryRulerInfo.Root.style.display = DisplayStyle.Flex;
            CountryRulerInfo.SetData(country, world);
        }
        else
        {
            CountryRulerInfo.Root.style.display = DisplayStyle.None;
        }

        buttonOrganize.SetEnabled(GameCore.Instance.StrategyActions.Organize.CanDo(chara));
        buttonHireVassal.SetEnabled(GameCore.Instance.StrategyActions.HireVassal.CanDo(chara));
        buttonFireVassal.SetEnabled(GameCore.Instance.StrategyActions.FireVassal.CanDo(chara));
        buttonAlly.SetEnabled(GameCore.Instance.StrategyActions.Ally.CanDo(chara));
        buttonResign.SetEnabled(GameCore.Instance.StrategyActions.Resign.CanDo(chara));
    }
}