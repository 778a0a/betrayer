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

public partial class SelectPlayerCaharacterScreen
{
    public Button buttonShowFreeList { get; private set; }

    public Button buttonRandom { get; private set; }

    public Label labelDescription { get; private set; }

    public VisualElement CellInformation { get; private set; }

    public CountryRulerInfo CountryRulerInfo { get; private set; }

    public CharacterTable CharacterTable { get; private set; }

    public CharacterInfo CharacterInfo { get; private set; }

    public VisualElement Root { get; }

    public SelectPlayerCaharacterScreen(VisualElement root)
    {
        Root = root;
        buttonShowFreeList = Root?.Q<Button>("buttonShowFreeList");
        buttonRandom = Root?.Q<Button>("buttonRandom");
        labelDescription = Root?.Q<Label>("labelDescription");
        CellInformation = Root?.Q<VisualElement>("CellInformation");
        CountryRulerInfo = new CountryRulerInfo(Root?.Q<VisualElement>("CountryRulerInfo"));
        CharacterTable = new CharacterTable(Root?.Q<VisualElement>("CharacterTable"));
        CharacterInfo = new CharacterInfo(Root?.Q<VisualElement>("CharacterInfo"));
    }
}