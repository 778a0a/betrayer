//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rosalina Code Generator tool.
//     Version: 4.0.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class IndividualPhaseScreen
{
    public Label labelTurnCount { get; private set; }

    public Button buttonShowInfo { get; private set; }

    public Button buttonHireSoldier { get; private set; }

    public Button buttonTrainSoldiers { get; private set; }

    public Button buttonGetJob { get; private set; }

    public Button buttonResign { get; private set; }

    public Button buttonRebel { get; private set; }

    public Button buttonBecomeIndependent { get; private set; }

    public Button buttonSeize { get; private set; }

    public Button buttonShowSystemMenu { get; private set; }

    public Button buttonEndTurn { get; private set; }

    public Label labelCost { get; private set; }

    public Label labelGold { get; private set; }

    public VisualElement ActionCostPanel { get; private set; }

    public VisualElement GoldPanel { get; private set; }

    public Image imageChara { get; private set; }

    public Label labelTitle { get; private set; }

    public Label labelName { get; private set; }

    public Label labelSalaryRatio { get; private set; }

    public Label labelCharaAttack { get; private set; }

    public Label labelCharaDefense { get; private set; }

    public Label labelCharaIntelligence { get; private set; }

    public Label labelCharaLoyalty { get; private set; }

    public Label labelCharaPrestige { get; private set; }

    public Label labelCharaContribution { get; private set; }

    public SoldierInfoLarge soldier00 { get; private set; }

    public SoldierInfoLarge soldier01 { get; private set; }

    public SoldierInfoLarge soldier02 { get; private set; }

    public SoldierInfoLarge soldier03 { get; private set; }

    public SoldierInfoLarge soldier04 { get; private set; }

    public SoldierInfoLarge soldier05 { get; private set; }

    public SoldierInfoLarge soldier06 { get; private set; }

    public SoldierInfoLarge soldier07 { get; private set; }

    public SoldierInfoLarge soldier08 { get; private set; }

    public SoldierInfoLarge soldier09 { get; private set; }

    public SoldierInfoLarge soldier10 { get; private set; }

    public SoldierInfoLarge soldier11 { get; private set; }

    public SoldierInfoLarge soldier12 { get; private set; }

    public SoldierInfoLarge soldier13 { get; private set; }

    public SoldierInfoLarge soldier14 { get; private set; }

    public VisualElement Soldiers { get; private set; }

    public Label labelHint { get; private set; }

    public VisualElement CharaPanel { get; private set; }

    public VisualElement ActionsPanel { get; private set; }

    public CountryRulerInfo CountryRulerInfo { get; private set; }

    public VisualElement Root { get; }

    public IndividualPhaseScreen(VisualElement root)
    {
        Root = root;
        labelTurnCount = Root?.Q<Label>("labelTurnCount");
        buttonShowInfo = Root?.Q<Button>("buttonShowInfo");
        buttonHireSoldier = Root?.Q<Button>("buttonHireSoldier");
        buttonTrainSoldiers = Root?.Q<Button>("buttonTrainSoldiers");
        buttonGetJob = Root?.Q<Button>("buttonGetJob");
        buttonResign = Root?.Q<Button>("buttonResign");
        buttonRebel = Root?.Q<Button>("buttonRebel");
        buttonBecomeIndependent = Root?.Q<Button>("buttonBecomeIndependent");
        buttonSeize = Root?.Q<Button>("buttonSeize");
        buttonShowSystemMenu = Root?.Q<Button>("buttonShowSystemMenu");
        buttonEndTurn = Root?.Q<Button>("buttonEndTurn");
        labelCost = Root?.Q<Label>("labelCost");
        labelGold = Root?.Q<Label>("labelGold");
        ActionCostPanel = Root?.Q<VisualElement>("ActionCostPanel");
        GoldPanel = Root?.Q<VisualElement>("GoldPanel");
        imageChara = Root?.Q<Image>("imageChara");
        labelTitle = Root?.Q<Label>("labelTitle");
        labelName = Root?.Q<Label>("labelName");
        labelSalaryRatio = Root?.Q<Label>("labelSalaryRatio");
        labelCharaAttack = Root?.Q<Label>("labelCharaAttack");
        labelCharaDefense = Root?.Q<Label>("labelCharaDefense");
        labelCharaIntelligence = Root?.Q<Label>("labelCharaIntelligence");
        labelCharaLoyalty = Root?.Q<Label>("labelCharaLoyalty");
        labelCharaPrestige = Root?.Q<Label>("labelCharaPrestige");
        labelCharaContribution = Root?.Q<Label>("labelCharaContribution");
        soldier00 = new SoldierInfoLarge(Root?.Q<VisualElement>("soldier00"));
        soldier01 = new SoldierInfoLarge(Root?.Q<VisualElement>("soldier01"));
        soldier02 = new SoldierInfoLarge(Root?.Q<VisualElement>("soldier02"));
        soldier03 = new SoldierInfoLarge(Root?.Q<VisualElement>("soldier03"));
        soldier04 = new SoldierInfoLarge(Root?.Q<VisualElement>("soldier04"));
        soldier05 = new SoldierInfoLarge(Root?.Q<VisualElement>("soldier05"));
        soldier06 = new SoldierInfoLarge(Root?.Q<VisualElement>("soldier06"));
        soldier07 = new SoldierInfoLarge(Root?.Q<VisualElement>("soldier07"));
        soldier08 = new SoldierInfoLarge(Root?.Q<VisualElement>("soldier08"));
        soldier09 = new SoldierInfoLarge(Root?.Q<VisualElement>("soldier09"));
        soldier10 = new SoldierInfoLarge(Root?.Q<VisualElement>("soldier10"));
        soldier11 = new SoldierInfoLarge(Root?.Q<VisualElement>("soldier11"));
        soldier12 = new SoldierInfoLarge(Root?.Q<VisualElement>("soldier12"));
        soldier13 = new SoldierInfoLarge(Root?.Q<VisualElement>("soldier13"));
        soldier14 = new SoldierInfoLarge(Root?.Q<VisualElement>("soldier14"));
        Soldiers = Root?.Q<VisualElement>("Soldiers");
        labelHint = Root?.Q<Label>("labelHint");
        CharaPanel = Root?.Q<VisualElement>("CharaPanel");
        ActionsPanel = Root?.Q<VisualElement>("ActionsPanel");
        CountryRulerInfo = new CountryRulerInfo(Root?.Q<VisualElement>("CountryRulerInfo"));
    }
}