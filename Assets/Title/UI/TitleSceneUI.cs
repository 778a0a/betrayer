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
                SaveDataSlotNo = currentSelectedSlotNo,
            });
        };

        // テキストデータ読み込み
        buttonLoadTextData.clicked += () =>
        {
            NewGameMenu.style.display = DisplayStyle.None;
            SaveDataManager.Instance.LoadTextData(currentSelectedSlotNo);
        };

        // スロットからコピー
        for (var i = 0; i < slotNoList.Length; i++)
        {
            var slotNo = slotNoList[i];
            var button = CopySlotButtons[i];
            button.clicked += () =>
            {
                NewGameMenu.style.display = DisplayStyle.None;
                SaveDataManager.Instance.Copy(slotNo, currentSelectedSlotNo);
            };
        }

        // オートセーブスロットからコピー
        buttonCopyFromSlotAuto.clicked += () =>
        {
            NewGameMenu.style.display = DisplayStyle.None;
            SaveDataManager.Instance.Copy(SaveDataManager.AutoSaveDataSlotNo, currentSelectedSlotNo);
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
