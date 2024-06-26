using System;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SystemWindow : IWindow
{
    public void Initialize()
    {
        Root.style.display = DisplayStyle.None;

        var cleared = GameCore.GameCleared;
        buttonChangeCharacter.enabledSelf = cleared;
        if (!cleared)
        {
            buttonChangeCharacter.text += " (クリア後解放)";
            buttonChangeCharacter.style.fontSize = 30;
        }

        buttonSave.clicked += () =>
        {
            try
            {
                Debug.Log("セーブします。");
                var core = GameCore.Instance;
                SaveDataManager.Instance.Save(core.SaveDataSlotNo, core);
                MessageWindow.Show("セーブしました。");
            }
            catch (Exception ex)
            {
                MessageWindow.Show($"セーブに失敗しました。\n({ex.Message})");
                Debug.LogError($"セーブに失敗しました。{ex}");
            }
        };

        buttonChangeCharacter.clicked += () =>
        {
            Util.Todo("キャラクター変更");
        };

        buttonGoToTitle.clicked += async () =>
        {
            var res = await MessageWindow.Show("タイトルに戻ります。\nよろしいですか？", MessageBoxButton.OkCancel);
            if (res != MessageBoxResult.Ok) return;
            TitleSceneManager.LoadScene();
        };

        buttonCloseSystemWindow.clicked += () =>
        {
            Root.style.display = DisplayStyle.None;
        };
    }

    public void Show()
    {
        Root.style.display = DisplayStyle.Flex;
    }
}