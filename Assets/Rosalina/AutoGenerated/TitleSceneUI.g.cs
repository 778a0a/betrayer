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

    public Button buttonCloseNewGameWindow { get; private set; }

    public Button buttonStartNewGame { get; private set; }

    public Button buttonLoadTextData { get; private set; }

    public Button buttonCopyFromSlot1 { get; private set; }

    public Button buttonCopyFromSlot2 { get; private set; }

    public Button buttonCopyFromSlot3 { get; private set; }

    public Button buttonCopyFromSlotAuto { get; private set; }

    public Button buttonClearText { get; private set; }

    public Button buttonPasteText { get; private set; }

    public Label labelTextBoxWindowTitle { get; private set; }

    public Button buttonCloseTextBoxWindow { get; private set; }

    public TextField textTextBoxWindow { get; private set; }

    public Button buttonCopyText { get; private set; }

    public Button buttonSubmitText { get; private set; }

    public Button buttonMessageOK { get; private set; }

    public Button buttonMessageYes { get; private set; }

    public Button buttonMessageNo { get; private set; }

    public Button buttonMessageCancel { get; private set; }

    public Label labelMessageText { get; private set; }

    public VisualElement NewGameMenu { get; private set; }

    public VisualElement TextBoxWindow { get; private set; }

    public VisualElement MessageWindow { get; private set; }

    public VisualElement Windows { get; private set; }

    public VisualElement Root => _document?.rootVisualElement;
    public void InitializeDocument()
    {
        SaveDataList = new SaveDataListWindow(Root?.Q<VisualElement>("SaveDataList"));
        buttonCloseApplication = Root?.Q<Button>("buttonCloseApplication");
        buttonCloseNewGameWindow = Root?.Q<Button>("buttonCloseNewGameWindow");
        buttonStartNewGame = Root?.Q<Button>("buttonStartNewGame");
        buttonLoadTextData = Root?.Q<Button>("buttonLoadTextData");
        buttonCopyFromSlot1 = Root?.Q<Button>("buttonCopyFromSlot1");
        buttonCopyFromSlot2 = Root?.Q<Button>("buttonCopyFromSlot2");
        buttonCopyFromSlot3 = Root?.Q<Button>("buttonCopyFromSlot3");
        buttonCopyFromSlotAuto = Root?.Q<Button>("buttonCopyFromSlotAuto");
        buttonClearText = Root?.Q<Button>("buttonClearText");
        buttonPasteText = Root?.Q<Button>("buttonPasteText");
        labelTextBoxWindowTitle = Root?.Q<Label>("labelTextBoxWindowTitle");
        buttonCloseTextBoxWindow = Root?.Q<Button>("buttonCloseTextBoxWindow");
        textTextBoxWindow = Root?.Q<TextField>("textTextBoxWindow");
        buttonCopyText = Root?.Q<Button>("buttonCopyText");
        buttonSubmitText = Root?.Q<Button>("buttonSubmitText");
        buttonMessageOK = Root?.Q<Button>("buttonMessageOK");
        buttonMessageYes = Root?.Q<Button>("buttonMessageYes");
        buttonMessageNo = Root?.Q<Button>("buttonMessageNo");
        buttonMessageCancel = Root?.Q<Button>("buttonMessageCancel");
        labelMessageText = Root?.Q<Label>("labelMessageText");
        NewGameMenu = Root?.Q<VisualElement>("NewGameMenu");
        TextBoxWindow = Root?.Q<VisualElement>("TextBoxWindow");
        MessageWindow = Root?.Q<VisualElement>("MessageWindow");
        Windows = Root?.Q<VisualElement>("Windows");
    }
}