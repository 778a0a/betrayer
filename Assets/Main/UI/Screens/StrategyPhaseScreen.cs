using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class StrategyPhaseScreen : IScreen
{
    public event EventHandler<ActionButtonHelper> ActionButtonClicked;

    private ActionButtonHelper[] buttons;

    public void Initialize()
    {
        buttons = new[]
        {
            ActionButtonHelper.Common(buttonShowInfo, a => a.ShowInfo),
            ActionButtonHelper.Strategy(buttonOrganize, a => a.Organize),
            ActionButtonHelper.Strategy(buttonHireVassal, a => a.HireVassal),
            ActionButtonHelper.Strategy(buttonFireVassal, a => a.FireVassal),
            ActionButtonHelper.Strategy(buttonAlly, a => a.Ally),
            ActionButtonHelper.Strategy(buttonChangeAllianceStance, a => a.ChangeAllianceStance),
            ActionButtonHelper.Strategy(buttonResign, a => a.Resign),
            ActionButtonHelper.Common(buttonShowSystemMenu, a => a.ShowSystemMenu),
            ActionButtonHelper.Common(buttonEndTurn, a => a.EndPhase),
        };
        foreach (var button in buttons)
        {
            button.SetEventHandlers(labelCost,
                () => debugCurrentChara,
                b =>ActionButtonClicked?.Invoke(this, b));
        }
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
        labelCharaLoyalty.text = chara.GetLoyaltyText(world);
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

        labelCost.text = "---";
        foreach (var button in buttons)
        {
            button.SetData(chara);
        }
    }
}