using System;
using System.Linq;
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

        buttonChangeCharacter.clicked += async () =>
        {
            var res = await MessageWindow.Show("操作キャラを変更します。\nよろしいですか？", MessageBoxButton.OkCancel);
            if (res != MessageBoxResult.Ok) return;

            var core = GameCore.Instance;
            var world = core.World;
            foreach (var chara in world.Characters.Where(c => c.IsPlayer))
            {
                chara.IsPlayer = false;
            }

            Root.style.display = DisplayStyle.None;
            ShowSelectPlayerCharacterUI(core);
        };

        buttonGoToTitle.clicked += async () =>
        {
            var res = await MessageWindow.Show("タイトル画面に戻ります。\nよろしいですか？", MessageBoxButton.OkCancel);
            if (res != MessageBoxResult.Ok) return;
            TitleSceneManager.LoadScene();
        };

        buttonCloseSystemWindow.clicked += () =>
        {
            Root.style.display = DisplayStyle.None;
        };
    }

    public static void ShowSelectPlayerCharacterUI(GameCore core)
    {
        var currentScreen = core.MainUI.currentScreen;
        GameCore.Instance.MainUI.ShowSelectPlayerCharacterUI(core.World, chara =>
        {
            // 観戦モード
            if (chara == null)
            {
                Debug.Log("観戦モードが選択されました。");
                core.IsWatchMode = true;
                Test.Instance.hold = false;
                core.MainUI.WatchModeWindow.Show();
            }
            else
            {
                chara.IsPlayer = true;
                Debug.Log($"Player selected: {chara.Name}");
                foreach (var c in core.World.Characters)
                {
                    c.AddUrami(-10000);
                }
                Test.Instance.hold = false;
                core.MainUI.ShowScreen(currentScreen);
            }
        });
    }

    public void Show()
    {
        Root.style.display = DisplayStyle.Flex;
    }
}