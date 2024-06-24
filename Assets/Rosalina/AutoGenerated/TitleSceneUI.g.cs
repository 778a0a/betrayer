//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rosalina Code Generator tool.
//     Version: 4.0.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UIElements;

public partial class TitleSceneUI
{
    [SerializeField]
    private UIDocument _document;
    public SaveDataListWindow SaveDataList { get; private set; }

    public Button buttonCloseApplication { get; private set; }

    public Label Label { get; private set; }

    public Button buttonCloseNewGameWindow { get; private set; }

    public Button buttonStartNewGame { get; private set; }

    public Button buttonLoadTextData { get; private set; }

    public Button buttonCopyFromSlot1 { get; private set; }

    public Button buttonCopyFromSlot2 { get; private set; }

    public Button buttonCopyFromSlot3 { get; private set; }

    public Button buttonCopyFromSlotAuto { get; private set; }

    public VisualElement NewGameMenu { get; private set; }

    public VisualElement Windows { get; private set; }

    public VisualElement Root => _document?.rootVisualElement;
    public void InitializeDocument()
    {
        SaveDataList = new SaveDataListWindow(Root?.Q<VisualElement>("SaveDataList"));
        buttonCloseApplication = Root?.Q<Button>("buttonCloseApplication");
        Label = Root?.Q<Label>("Label");
        buttonCloseNewGameWindow = Root?.Q<Button>("buttonCloseNewGameWindow");
        buttonStartNewGame = Root?.Q<Button>("buttonStartNewGame");
        buttonLoadTextData = Root?.Q<Button>("buttonLoadTextData");
        buttonCopyFromSlot1 = Root?.Q<Button>("buttonCopyFromSlot1");
        buttonCopyFromSlot2 = Root?.Q<Button>("buttonCopyFromSlot2");
        buttonCopyFromSlot3 = Root?.Q<Button>("buttonCopyFromSlot3");
        buttonCopyFromSlotAuto = Root?.Q<Button>("buttonCopyFromSlotAuto");
        NewGameMenu = Root?.Q<VisualElement>("NewGameMenu");
        Windows = Root?.Q<VisualElement>("Windows");
    }
}