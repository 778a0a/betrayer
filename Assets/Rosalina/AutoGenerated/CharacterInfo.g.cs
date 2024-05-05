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

public partial class CharacterInfo
{
    public Label labelCharaStatus { get; private set; }

    public Label labelCharaName { get; private set; }

    public Label labelCharaAttack { get; private set; }

    public Label labelCharaDefense { get; private set; }

    public Label labelCharaIntelligence { get; private set; }

    public Label labelCharaLoyalty { get; private set; }

    public Label labelCharaPrestige { get; private set; }

    public Label labelCharaContribution { get; private set; }

    public Label labelCharaSalaryRatio { get; private set; }

    public CharacterInfoSoldierIcon soldier00 { get; private set; }

    public CharacterInfoSoldierIcon soldier01 { get; private set; }

    public CharacterInfoSoldierIcon soldier02 { get; private set; }

    public CharacterInfoSoldierIcon soldier03 { get; private set; }

    public CharacterInfoSoldierIcon soldier04 { get; private set; }

    public CharacterInfoSoldierIcon soldier05 { get; private set; }

    public CharacterInfoSoldierIcon soldier06 { get; private set; }

    public CharacterInfoSoldierIcon soldier07 { get; private set; }

    public CharacterInfoSoldierIcon soldier08 { get; private set; }

    public CharacterInfoSoldierIcon soldier09 { get; private set; }

    public VisualElement Soldiers { get; private set; }

    public VisualElement VisualElement { get; private set; }

    public Image imageChara { get; private set; }

    public VisualElement Root { get; }

    public CharacterInfo(VisualElement root)
    {
        Root = root;
        labelCharaStatus = Root?.Q<Label>("labelCharaStatus");
        labelCharaName = Root?.Q<Label>("labelCharaName");
        labelCharaAttack = Root?.Q<Label>("labelCharaAttack");
        labelCharaDefense = Root?.Q<Label>("labelCharaDefense");
        labelCharaIntelligence = Root?.Q<Label>("labelCharaIntelligence");
        labelCharaLoyalty = Root?.Q<Label>("labelCharaLoyalty");
        labelCharaPrestige = Root?.Q<Label>("labelCharaPrestige");
        labelCharaContribution = Root?.Q<Label>("labelCharaContribution");
        labelCharaSalaryRatio = Root?.Q<Label>("labelCharaSalaryRatio");
        soldier00 = new CharacterInfoSoldierIcon(Root?.Q<VisualElement>("soldier00"));
        soldier01 = new CharacterInfoSoldierIcon(Root?.Q<VisualElement>("soldier01"));
        soldier02 = new CharacterInfoSoldierIcon(Root?.Q<VisualElement>("soldier02"));
        soldier03 = new CharacterInfoSoldierIcon(Root?.Q<VisualElement>("soldier03"));
        soldier04 = new CharacterInfoSoldierIcon(Root?.Q<VisualElement>("soldier04"));
        soldier05 = new CharacterInfoSoldierIcon(Root?.Q<VisualElement>("soldier05"));
        soldier06 = new CharacterInfoSoldierIcon(Root?.Q<VisualElement>("soldier06"));
        soldier07 = new CharacterInfoSoldierIcon(Root?.Q<VisualElement>("soldier07"));
        soldier08 = new CharacterInfoSoldierIcon(Root?.Q<VisualElement>("soldier08"));
        soldier09 = new CharacterInfoSoldierIcon(Root?.Q<VisualElement>("soldier09"));
        Soldiers = Root?.Q<VisualElement>("Soldiers");
        VisualElement = Root?.Q<VisualElement>("VisualElement");
        imageChara = Root?.Q<Image>("imageChara");
    }
}