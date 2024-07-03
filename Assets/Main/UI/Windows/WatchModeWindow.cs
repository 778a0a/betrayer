using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class WatchModeWindow : IWindow
{
    public bool IsVisible { get; private set; }

    public void Initialize()
    {
        Hide();
        buttonChangeCharacter.clicked += () =>
        {
            buttonChangeCharacter.text = "ターン終了までお待ちください...";
            buttonChangeCharacter.enabledSelf = false;
            Test.Instance.hold = true;
            SystemWindow.ShowSelectPlayerCharacterUI(GameCore.Instance);
        };
    }

    public void Show()
    {
        buttonChangeCharacter.text = "操作キャラ変更";
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