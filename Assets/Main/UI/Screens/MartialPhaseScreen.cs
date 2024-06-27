using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class MartialPhaseScreen : IScreen
{
    public event EventHandler<ActionButtonHelper> ActionButtonClicked;

    private ActionButtonHelper[] buttons;

    public void Initialize()
    {
        buttons = new[]
        {
            ActionButtonHelper.Common(buttonShowInfo, a => a.ShowInfo),
            ActionButtonHelper.Martial(buttonAttack, a => a.Attack),
            ActionButtonHelper.Martial(buttonDecisiveBattle, a => a.DecisiveBattle),
            ActionButtonHelper.Martial(buttonProvoke, a => a.Provoke),
            ActionButtonHelper.Martial(buttonSubdue, a => a.Subdue),
            ActionButtonHelper.Martial(buttonPrivateFight, a => a.PrivateFight),
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
    public Character debugCurrentChara;
    public void SetData(Character chara, WorldData world)
    {
        debugCurrentChara = chara;
        imageChara.image = FaceImageManager.Instance.GetImage(chara);
        labelName.text = chara.Name;
        labelTitle.text = chara.GetTitle(world);
        labelSalaryRatio.text = chara.SalaryRatio.ToString();

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

        labelCost.text = "---";
        foreach (var button in buttons)
        {
            button.SetData(chara);
        }
    }
}