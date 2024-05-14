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

public partial class BattleDialog
{
    public Label AttackerName { get; private set; }

    public Label DefenderName { get; private set; }

    public BattleSoldierIcon AttackerSoldier10 { get; private set; }

    public BattleSoldierIcon AttackerSoldier11 { get; private set; }

    public BattleSoldierIcon AttackerSoldier12 { get; private set; }

    public BattleSoldierIcon AttackerSoldier13 { get; private set; }

    public BattleSoldierIcon AttackerSoldier14 { get; private set; }

    public BattleSoldierIcon AttackerSoldier05 { get; private set; }

    public BattleSoldierIcon AttackerSoldier06 { get; private set; }

    public BattleSoldierIcon AttackerSoldier07 { get; private set; }

    public BattleSoldierIcon AttackerSoldier08 { get; private set; }

    public BattleSoldierIcon AttackerSoldier09 { get; private set; }

    public BattleSoldierIcon AttackerSoldier00 { get; private set; }

    public BattleSoldierIcon AttackerSoldier01 { get; private set; }

    public BattleSoldierIcon AttackerSoldier02 { get; private set; }

    public BattleSoldierIcon AttackerSoldier03 { get; private set; }

    public BattleSoldierIcon AttackerSoldier04 { get; private set; }

    public BattleSoldierIcon DefenderSoldier00 { get; private set; }

    public BattleSoldierIcon DefenderSoldier01 { get; private set; }

    public BattleSoldierIcon DefenderSoldier02 { get; private set; }

    public BattleSoldierIcon DefenderSoldier03 { get; private set; }

    public BattleSoldierIcon DefenderSoldier04 { get; private set; }

    public BattleSoldierIcon DefenderSoldier05 { get; private set; }

    public BattleSoldierIcon DefenderSoldier06 { get; private set; }

    public BattleSoldierIcon DefenderSoldier07 { get; private set; }

    public BattleSoldierIcon DefenderSoldier08 { get; private set; }

    public BattleSoldierIcon DefenderSoldier09 { get; private set; }

    public BattleSoldierIcon DefenderSoldier10 { get; private set; }

    public BattleSoldierIcon DefenderSoldier11 { get; private set; }

    public BattleSoldierIcon DefenderSoldier12 { get; private set; }

    public BattleSoldierIcon DefenderSoldier13 { get; private set; }

    public BattleSoldierIcon DefenderSoldier14 { get; private set; }

    public VisualElement AttackerSoldiers { get; private set; }

    public VisualElement DefenderSoldiers { get; private set; }

    public Label labelAttackerAttack { get; private set; }

    public Label labelAttackerIntelligense { get; private set; }

    public VisualElement imageAttacker { get; private set; }

    public Label labelAttackerTerrain { get; private set; }

    public Label labelDefenderDefence { get; private set; }

    public Label labelDefenderIntelligense { get; private set; }

    public VisualElement imageDefender { get; private set; }

    public Label labelDefenderTerrain { get; private set; }

    public VisualElement AttackerInfo { get; private set; }

    public VisualElement DefenderInfo { get; private set; }

    public Button buttonAttack { get; private set; }

    public Button buttonRetreat { get; private set; }

    public VisualElement VisualElement { get; private set; }

    public VisualElement Root { get; }

    public BattleDialog(VisualElement root)
    {
        Root = root;
        AttackerName = Root?.Q<Label>("AttackerName");
        DefenderName = Root?.Q<Label>("DefenderName");
        AttackerSoldier10 = new BattleSoldierIcon(Root?.Q<VisualElement>("AttackerSoldier10"));
        AttackerSoldier11 = new BattleSoldierIcon(Root?.Q<VisualElement>("AttackerSoldier11"));
        AttackerSoldier12 = new BattleSoldierIcon(Root?.Q<VisualElement>("AttackerSoldier12"));
        AttackerSoldier13 = new BattleSoldierIcon(Root?.Q<VisualElement>("AttackerSoldier13"));
        AttackerSoldier14 = new BattleSoldierIcon(Root?.Q<VisualElement>("AttackerSoldier14"));
        AttackerSoldier05 = new BattleSoldierIcon(Root?.Q<VisualElement>("AttackerSoldier05"));
        AttackerSoldier06 = new BattleSoldierIcon(Root?.Q<VisualElement>("AttackerSoldier06"));
        AttackerSoldier07 = new BattleSoldierIcon(Root?.Q<VisualElement>("AttackerSoldier07"));
        AttackerSoldier08 = new BattleSoldierIcon(Root?.Q<VisualElement>("AttackerSoldier08"));
        AttackerSoldier09 = new BattleSoldierIcon(Root?.Q<VisualElement>("AttackerSoldier09"));
        AttackerSoldier00 = new BattleSoldierIcon(Root?.Q<VisualElement>("AttackerSoldier00"));
        AttackerSoldier01 = new BattleSoldierIcon(Root?.Q<VisualElement>("AttackerSoldier01"));
        AttackerSoldier02 = new BattleSoldierIcon(Root?.Q<VisualElement>("AttackerSoldier02"));
        AttackerSoldier03 = new BattleSoldierIcon(Root?.Q<VisualElement>("AttackerSoldier03"));
        AttackerSoldier04 = new BattleSoldierIcon(Root?.Q<VisualElement>("AttackerSoldier04"));
        DefenderSoldier00 = new BattleSoldierIcon(Root?.Q<VisualElement>("DefenderSoldier00"));
        DefenderSoldier01 = new BattleSoldierIcon(Root?.Q<VisualElement>("DefenderSoldier01"));
        DefenderSoldier02 = new BattleSoldierIcon(Root?.Q<VisualElement>("DefenderSoldier02"));
        DefenderSoldier03 = new BattleSoldierIcon(Root?.Q<VisualElement>("DefenderSoldier03"));
        DefenderSoldier04 = new BattleSoldierIcon(Root?.Q<VisualElement>("DefenderSoldier04"));
        DefenderSoldier05 = new BattleSoldierIcon(Root?.Q<VisualElement>("DefenderSoldier05"));
        DefenderSoldier06 = new BattleSoldierIcon(Root?.Q<VisualElement>("DefenderSoldier06"));
        DefenderSoldier07 = new BattleSoldierIcon(Root?.Q<VisualElement>("DefenderSoldier07"));
        DefenderSoldier08 = new BattleSoldierIcon(Root?.Q<VisualElement>("DefenderSoldier08"));
        DefenderSoldier09 = new BattleSoldierIcon(Root?.Q<VisualElement>("DefenderSoldier09"));
        DefenderSoldier10 = new BattleSoldierIcon(Root?.Q<VisualElement>("DefenderSoldier10"));
        DefenderSoldier11 = new BattleSoldierIcon(Root?.Q<VisualElement>("DefenderSoldier11"));
        DefenderSoldier12 = new BattleSoldierIcon(Root?.Q<VisualElement>("DefenderSoldier12"));
        DefenderSoldier13 = new BattleSoldierIcon(Root?.Q<VisualElement>("DefenderSoldier13"));
        DefenderSoldier14 = new BattleSoldierIcon(Root?.Q<VisualElement>("DefenderSoldier14"));
        AttackerSoldiers = Root?.Q<VisualElement>("AttackerSoldiers");
        DefenderSoldiers = Root?.Q<VisualElement>("DefenderSoldiers");
        labelAttackerAttack = Root?.Q<Label>("labelAttackerAttack");
        labelAttackerIntelligense = Root?.Q<Label>("labelAttackerIntelligense");
        imageAttacker = Root?.Q<VisualElement>("imageAttacker");
        labelAttackerTerrain = Root?.Q<Label>("labelAttackerTerrain");
        labelDefenderDefence = Root?.Q<Label>("labelDefenderDefence");
        labelDefenderIntelligense = Root?.Q<Label>("labelDefenderIntelligense");
        imageDefender = Root?.Q<VisualElement>("imageDefender");
        labelDefenderTerrain = Root?.Q<Label>("labelDefenderTerrain");
        AttackerInfo = Root?.Q<VisualElement>("AttackerInfo");
        DefenderInfo = Root?.Q<VisualElement>("DefenderInfo");
        buttonAttack = Root?.Q<Button>("buttonAttack");
        buttonRetreat = Root?.Q<Button>("buttonRetreat");
        VisualElement = Root?.Q<VisualElement>("VisualElement");
    }
}