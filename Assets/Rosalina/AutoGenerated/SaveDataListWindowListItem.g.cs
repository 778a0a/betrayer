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

public partial class SaveDataListWindowListItem
{
    public Label labelSoldiers { get; private set; }

    public Label labelTurnCount { get; private set; }

    public Label labelTitle { get; private set; }

    public Label labelName { get; private set; }

    public Label labelSavedTime { get; private set; }

    public Image imageCharacter { get; private set; }

    public Button buttonMain { get; private set; }

    public Button buttonDownload { get; private set; }

    public Button buttonDelete { get; private set; }

    public VisualElement SaveDataLisItemRoot { get; private set; }

    public Button buttonNoData { get; private set; }

    public VisualElement Root { get; }

    public SaveDataListWindowListItem(VisualElement root)
    {
        Root = root;
        labelSoldiers = Root?.Q<Label>("labelSoldiers");
        labelTurnCount = Root?.Q<Label>("labelTurnCount");
        labelTitle = Root?.Q<Label>("labelTitle");
        labelName = Root?.Q<Label>("labelName");
        labelSavedTime = Root?.Q<Label>("labelSavedTime");
        imageCharacter = Root?.Q<Image>("imageCharacter");
        buttonMain = Root?.Q<Button>("buttonMain");
        buttonDownload = Root?.Q<Button>("buttonDownload");
        buttonDelete = Root?.Q<Button>("buttonDelete");
        SaveDataLisItemRoot = Root?.Q<VisualElement>("SaveDataLisItemRoot");
        buttonNoData = Root?.Q<Button>("buttonNoData");
    }
}