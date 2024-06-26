using System;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SystemWindow : IDialog
{
    public void Initialize()
    {
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
                Util.Todo("保存完了通知");
            }
            catch (Exception ex)
            {
                Util.Todo("エラー通知");
                Debug.LogError($"セーブに失敗しました。{ex}");
            }
        };

        buttonChangeCharacter.clicked += () =>
        {
            Util.Todo("キャラクター変更");
        };

        buttonGoToTitle.clicked += () =>
        {
            Util.Todo("確認");
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