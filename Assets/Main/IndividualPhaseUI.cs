using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public partial class IndividualPhaseUI : IScreen
{
    public event EventHandler<ActionButton> ActionButtonClicked;

    public enum ActionButton
    {
        ShowInfo,
        HireSoldier,
        TrainSoldiers,
        GetJob,
        Resign,
        Rebel,
        BecomeIndependent,
        ShowSystemMenu,
        EndTurn,
    }

    public void Initialize()
    {
        var buttons = new[]
        {
            buttonShowInfo,
            buttonHireSoldier,
            buttonTrainSoldiers,
            buttonGetJob,
            buttonResign,
            buttonRebel,
            buttonBecomeIndependent,
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
    public Character debugCurrentChara;
    public void SetData(Character chara, WorldData world)
    {
        debugCurrentChara = chara;
        imageChara.image = FaceImageManager.Instance.GetImage(chara);
        labelName.text = chara.Name;
        labelTitle.text = "将軍";
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

        buttonHireSoldier.SetEnabled(PersonalActions.HireSoldier.CanDo(chara));
        buttonTrainSoldiers.SetEnabled(PersonalActions.TrainSoldiers.CanDo(chara));
        buttonGetJob.SetEnabled(PersonalActions.GetJob.CanDo(chara));
        buttonResign.SetEnabled(PersonalActions.Resign.CanDo(chara));
        buttonRebel.SetEnabled(PersonalActions.Rebel.CanDo(chara));
        buttonBecomeIndependent.SetEnabled(PersonalActions.BecomeIndependent.CanDo(chara));
    }
}