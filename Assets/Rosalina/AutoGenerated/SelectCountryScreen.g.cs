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

public partial class SelectCountryScreen
{
    public Label labelDescription { get; private set; }

    public CountryRulerInfo CountryRulerInfo { get; private set; }

    public CharacterTable CharacterTable { get; private set; }

    public CharacterInfo CharacterInfo { get; private set; }

    public Button buttonSelect { get; private set; }

    public Button buttonClose { get; private set; }

    public VisualElement Root { get; }

    public SelectCountryScreen(VisualElement root)
    {
        Root = root;
        labelDescription = Root?.Q<Label>("labelDescription");
        CountryRulerInfo = new CountryRulerInfo(Root?.Q<VisualElement>("CountryRulerInfo"));
        CharacterTable = new CharacterTable(Root?.Q<VisualElement>("CharacterTable"));
        CharacterInfo = new CharacterInfo(Root?.Q<VisualElement>("CharacterInfo"));
        buttonSelect = Root?.Q<Button>("buttonSelect");
        buttonClose = Root?.Q<Button>("buttonClose");
    }
}