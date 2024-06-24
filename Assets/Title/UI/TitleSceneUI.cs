using System;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public partial class TitleSceneUI : MonoBehaviour
{
    private int currentSelectedSlotNo = 0;

    private void OnEnable()
    {
        InitializeDocument();
        NewGameMenu.style.display = DisplayStyle.None;

        InitializeNewGameWindow();
        SaveDataList.Initialize(this);

        buttonCloseApplication.clicked += () =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_EDITOR             
            Application.Quit();
#endif
        };

        SaveDataList.SetData(SaveDataManager.Instance);
    }

    private readonly int[] slotNoList = new[] { 0, 1, 2 };
    private Button[] CopySlotButtons => new[] { buttonCopyFromSlot1, buttonCopyFromSlot2, buttonCopyFromSlot3 };

    private void InitializeNewGameWindow()
    {
        // 閉じるボタン
        buttonCloseNewGameWindow.clicked += () => NewGameMenu.style.display = DisplayStyle.None;
        
        // はじめから
        buttonStartNewGame.clicked += () =>
        {
            NewGameMenu.style.display = DisplayStyle.None;
            MainSceneManager.LoadScene(new MainSceneStartArguments()
            {
                IsNewGame = true,
                NewGameSaveDataSlotNo = currentSelectedSlotNo,
            });
        };

        // テキストデータ読み込み
        buttonLoadTextData.clicked += () =>
        {
            NewGameMenu.style.display = DisplayStyle.None;
            var saveDataText = SaveDataManager.Instance.LoadFromClipboard();
            SaveDataManager.Instance.Save(currentSelectedSlotNo, saveDataText);
            SaveDataList.SetData(SaveDataManager.Instance);
            Util.Todo("ペースト用のウィンドウ表示");
        };

        // スロットからコピー
        for (var i = 0; i < slotNoList.Length; i++)
        {
            var slotNo = slotNoList[i];
            var button = CopySlotButtons[i];
            button.clicked += () =>
            {
                SaveDataManager.Instance.Copy(slotNo, currentSelectedSlotNo);
                SaveDataList.SetData(SaveDataManager.Instance);
                NewGameMenu.style.display = DisplayStyle.None;
            };
        }

        // オートセーブスロットからコピー
        buttonCopyFromSlotAuto.clicked += () =>
        {
            SaveDataManager.Instance.Copy(SaveDataManager.AutoSaveDataSlotNo, currentSelectedSlotNo);
            SaveDataList.SetData(SaveDataManager.Instance);
            NewGameMenu.style.display = DisplayStyle.None;
        };
    }

    public void ShowNewGameWindow(int selectedSlotNo)
    {
        currentSelectedSlotNo = selectedSlotNo;
        NewGameMenu.style.display = DisplayStyle.Flex;

        for (var i = 0; i < slotNoList.Length; i++)
        {
            var slotNo = slotNoList[i];
            var isSelectedSlot = slotNo == selectedSlotNo;
            var hasSaveData = SaveDataManager.Instance.HasSaveData(slotNo);
            CopySlotButtons[i].SetEnabled(!isSelectedSlot && hasSaveData);
        }
        
        var hasAutoSaveData = SaveDataManager.Instance.HasSaveData(SaveDataManager.AutoSaveDataSlotNo);
        buttonCopyFromSlotAuto.SetEnabled(hasAutoSaveData);
    }
}
