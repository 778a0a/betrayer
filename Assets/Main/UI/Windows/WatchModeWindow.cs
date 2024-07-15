using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class WatchModeWindow : IWindow
{
    private LocalizationManager L => MainUI.Instance.L;

    public bool IsVisible { get; private set; }

    public void Initialize()
    {
        Hide();
        L.Register(this);
        buttonChangeCharacter.clicked += () =>
        {
            buttonChangeCharacter.text = L["ターン終了までお待ちください..."];
            buttonChangeCharacter.enabledSelf = false;
            Test.Instance.hold = true;
            SystemWindow.ShowSelectPlayerCharacterUI(GameCore.Instance);
        };
    }

    public void Show()
    {
        buttonChangeCharacter.text = L["操作キャラ変更"];
        buttonChangeCharacter.enabledSelf = true;
        Root.style.display = DisplayStyle.Flex;
        IsVisible = true;
    }

    public void Hide()
    {
        Root.style.display = DisplayStyle.None;
        IsVisible = false;
    }
}