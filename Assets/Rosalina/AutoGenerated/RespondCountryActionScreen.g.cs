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

public partial class RespondCountryActionScreen
{
    public CountryRulerInfo CountryRulerInfo { get; private set; }

    public CharacterTable CharacterTable { get; private set; }

    public CharacterInfo CharacterInfo { get; private set; }

    public Button buttonYes { get; private set; }

    public Button buttonNo { get; private set; }

    public Label labelDescription { get; private set; }

    public VisualElement Root { get; }

    public RespondCountryActionScreen(VisualElement root)
    {
        Root = root;
        CountryRulerInfo = new CountryRulerInfo(Root?.Q<VisualElement>("CountryRulerInfo"));
        CharacterTable = new CharacterTable(Root?.Q<VisualElement>("CharacterTable"));
        CharacterInfo = new CharacterInfo(Root?.Q<VisualElement>("CharacterInfo"));
        buttonYes = Root?.Q<Button>("buttonYes");
        buttonNo = Root?.Q<Button>("buttonNo");
        labelDescription = Root?.Q<Label>("labelDescription");
    }
}