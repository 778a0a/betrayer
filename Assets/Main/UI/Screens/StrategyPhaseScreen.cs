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
        ChangeAllianceStance,
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
            buttonChangeAllianceStance,
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
        var country = world.CountryOf(chara);

        imageChara.image = FaceImageManager.Instance.GetImage(chara);
        labelName.text = chara.Name;
        labelTitle.text = chara.GetTitle(world);
        labelYearsOfService.text = "88";
        labelSalaryRatio.text = chara.SalaryRatio.ToString();

        if (world.IsRuler(chara) && country.Ally != null)
        {
            containerAllianceStance.style.display = DisplayStyle.Flex;
            var yes = country.WantsToContinueAlliance;
            var text = yes ? "継続" : "解消";
            var turns = country.TurnCountToDisableAlliance;
            labelAllianceStance.text = text + (yes ? "" : $"({turns})");
        }
        else
        {
            containerAllianceStance.style.display = DisplayStyle.None;
        }

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

        if (country != null)
        {
            CountryRulerInfo.Root.style.display = DisplayStyle.Flex;
            CountryRulerInfo.SetData(country, world);
        }
        else
        {
            CountryRulerInfo.Root.style.display = DisplayStyle.None;
        }

        var actions = GameCore.Instance.StrategyActions;
        IScreen.SetActionButton(buttonOrganize, actions.Organize, chara);
        IScreen.SetActionButton(buttonHireVassal, actions.HireVassal, chara);
        IScreen.SetActionButton(buttonFireVassal, actions.FireVassal, chara);
        IScreen.SetActionButton(buttonAlly, actions.Ally, chara);
        IScreen.SetActionButton(buttonChangeAllianceStance, actions.ChangeAllianceStance, chara);
        IScreen.SetActionButton(buttonResign, actions.Resign, chara);
    }
}