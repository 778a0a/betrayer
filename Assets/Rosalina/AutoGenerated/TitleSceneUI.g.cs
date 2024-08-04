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
    public Button buttonShowSystemSettings { get; private set; }

    public Button buttonShowLicense { get; private set; }

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

    public Label labelProgressWindowTitle { get; private set; }

    public ProgressBar progressLoading { get; private set; }

    public Label labelLicenseWindowTitle { get; private set; }

    public Button buttonCloseLicenseWindow { get; private set; }

    public TextField textLicenseWindow { get; private set; }

    public VisualElement NewGameMenu { get; private set; }

    public VisualElement TextBoxWindow { get; private set; }

    public VisualElement ProgressWindow { get; private set; }

    public VisualElement LicenseWindow { get; private set; }

    public SystemSettingsWindow SystemSettingsWindow { get; private set; }

    public MessageWindow MessageWindow { get; private set; }

    public VisualElement Windows { get; private set; }

    public VisualElement Root => _document?.rootVisualElement;
    public void InitializeDocument()
    {
        buttonShowSystemSettings = Root?.Q<Button>("buttonShowSystemSettings");
        buttonShowLicense = Root?.Q<Button>("buttonShowLicense");
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
        labelProgressWindowTitle = Root?.Q<Label>("labelProgressWindowTitle");
        progressLoading = Root?.Q<ProgressBar>("progressLoading");
        labelLicenseWindowTitle = Root?.Q<Label>("labelLicenseWindowTitle");
        buttonCloseLicenseWindow = Root?.Q<Button>("buttonCloseLicenseWindow");
        textLicenseWindow = Root?.Q<TextField>("textLicenseWindow");
        NewGameMenu = Root?.Q<VisualElement>("NewGameMenu");
        TextBoxWindow = Root?.Q<VisualElement>("TextBoxWindow");
        ProgressWindow = Root?.Q<VisualElement>("ProgressWindow");
        LicenseWindow = Root?.Q<VisualElement>("LicenseWindow");
        SystemSettingsWindow = new SystemSettingsWindow(Root?.Q<VisualElement>("SystemSettingsWindow"));
        MessageWindow = new MessageWindow(Root?.Q<VisualElement>("MessageWindow"));
        Windows = Root?.Q<VisualElement>("Windows");
    }
}