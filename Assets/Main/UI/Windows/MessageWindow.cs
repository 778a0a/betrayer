using System.Threading.Tasks;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class MessageWindow : IWindow
{
    public static MessageWindow Instance { get; private set; }

    private ValueTaskCompletionSource<MessageBoxResult> tcsMessageWindow;

    public void Initialize()
    {
        Instance = this;
        void OnClick(MessageBoxResult result)
        {
            tcsMessageWindow.SetResult(result);
            tcsMessageWindow = null;
            Root.style.display = DisplayStyle.None;
        }

        buttonMessageOK.clicked += () => OnClick(MessageBoxResult.Ok);
        buttonMessageYes.clicked += () => OnClick(MessageBoxResult.Yes);
        buttonMessageNo.clicked += () => OnClick(MessageBoxResult.No);
        buttonMessageCancel.clicked += () => OnClick(MessageBoxResult.Cancel);
        Root.style.display = DisplayStyle.None;
    }

    public static ValueTask<MessageBoxResult> Show(
        string message,
        MessageBoxButton button = MessageBoxButton.Ok,
        object _ = null) // インスタンスメソッドと同じシグネチャの宣言にしないためのダミー引数
        => Instance.Show(message, button);

    public ValueTask<MessageBoxResult> Show(
        string message,
        MessageBoxButton button = MessageBoxButton.Ok)
    {
        if (tcsMessageWindow != null) throw new InvalidOperationException();
        tcsMessageWindow = new();

        labelMessageText.text = message;
        buttonMessageOK.style.display = Util.Display(button.HasFlag(MessageBoxButton.Ok));
        buttonMessageYes.style.display = Util.Display(button.HasFlag(MessageBoxButton.Yes));
        buttonMessageNo.style.display = Util.Display(button.HasFlag(MessageBoxButton.No));
        buttonMessageCancel.style.display = Util.Display(button.HasFlag(MessageBoxButton.Cancel));

        Root.style.display = DisplayStyle.Flex;
        return tcsMessageWindow.Task;
    }
}

[Flags]
public enum MessageBoxButton
{
    Ok = 1 << 0,
    Yes = 1 << 1,
    No = 1 << 2,
    Cancel = 1 << 3,

    YesNo = Yes | No,
    YesNoCancel = Yes | No | Cancel,
    OkCancel = Ok | Cancel,
}

public enum MessageBoxResult
{
    Ok,
    Yes,
    No,
    Cancel,
}
