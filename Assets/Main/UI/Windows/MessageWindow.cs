using System.Threading.Tasks;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class MessageWindow : IWindow
{
    private ValueTaskCompletionSource<MessageBoxResult> tcsMessageWindow;

    public void Initialize()
    {
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

    public ValueTask<MessageBoxResult> Show(string message, MessageBoxButton button = MessageBoxButton.Ok)
    {
        if (tcsMessageWindow != null) throw new InvalidOperationException();
        tcsMessageWindow = new();

        labelMessageText.text = message;
        buttonMessageOK.style.display = Util.Display(button == MessageBoxButton.Ok);
        buttonMessageYes.style.display = Util.Display(button == MessageBoxButton.YesNo || button == MessageBoxButton.YesNoCancel);
        buttonMessageNo.style.display = Util.Display(button == MessageBoxButton.YesNo || button == MessageBoxButton.YesNoCancel);
        buttonMessageCancel.style.display = Util.Display(button == MessageBoxButton.YesNoCancel);

        Root.style.display = DisplayStyle.Flex;
        return tcsMessageWindow.Task;
    }
}

public enum MessageBoxButton
{
    Ok,
    YesNo,
    YesNoCancel,
}

public enum MessageBoxResult
{
    Ok,
    Yes,
    No,
    Cancel,
}
