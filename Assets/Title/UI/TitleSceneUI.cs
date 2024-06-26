using System;
using System.IO.Compression;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public partial class TitleSceneUI : MonoBehaviour
{
    private int currentSelectedSlotNo = 0;

    private void OnEnable()
    {
        InitializeDocument();
        InitializeNewGameWindow();
        InitializeTextBoxWindow();
        InitializeProgressWindow();
        InitializeMessageWindow();
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


    #region NewGameWindow
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
            var op = MainSceneManager.LoadScene(new MainSceneStartArguments()
            {
                IsNewGame = true,
                NewGameSaveDataSlotNo = currentSelectedSlotNo,
            });
            OnSceneLoadingStart(op);
        };

        // テキストデータ読み込み
        buttonLoadTextData.clicked += () =>
        {
            ShowTextBoxWindow(text =>
            {
                try
                {
                    var saveDataText = SaveDataText.FromPlainText(text);
                    
                    // セーブデータのスロット番号を書き換える。
                    var saveData = saveDataText.Deserialize();
                    saveData.Summary.SaveDataSlotNo = currentSelectedSlotNo;
                    saveDataText = SaveDataText.Serialize(saveData);

                    SaveDataManager.Instance.Save(currentSelectedSlotNo, saveDataText);
                    SaveDataList.SetData(SaveDataManager.Instance);
                    TextBoxWindow.style.display = DisplayStyle.None;
                    NewGameMenu.style.display = DisplayStyle.None;
                }
                catch (Exception ex)
                {
                    ShowMessageWindow($"セーブデータの読み込みに失敗しました。\n({ex.Message})");
                    Debug.LogError($"セーブデータの読み込みに失敗しました。 {ex}");
                }
            }, isCopy: false);
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

        NewGameMenu.style.display = DisplayStyle.None;
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
    #endregion

    #region TextBoxWindow
    private Action<string> onTextSubmit;

    private void InitializeTextBoxWindow()
    {
        buttonCloseTextBoxWindow.clicked += () =>
        {
            TextBoxWindow.style.display = DisplayStyle.None;
        };
        buttonClearText.clicked += () =>
        {
            textTextBoxWindow.value = "";
        };
        buttonPasteText.clicked += () =>
        {
            try
            {
                textTextBoxWindow.value = GUIUtility.systemCopyBuffer;
            }
            catch (Exception ex)
            {
                ShowMessageWindow($"クリップボードからの貼り付けに失敗しました。\n({ex.Message})");
                Debug.LogError($"クリップボードからの貼り付けに失敗しました。 {ex}");
            }
        };
        buttonCopyText.clicked += () =>
        {
            try
            {
                GUIUtility.systemCopyBuffer = textTextBoxWindow.value;
            }
            catch (Exception ex)
            {
                ShowMessageWindow($"クリップボードへのコピーに失敗しました。\n({ex.Message})");
                Debug.LogError($"クリップボードへのコピーに失敗しました。 {ex}");
            }
        };
        buttonSubmitText.clicked += () =>
        {
            var text = textTextBoxWindow.value;
            onTextSubmit?.Invoke(text);
            if (onTextSubmit == null)
            {
                TextBoxWindow.style.display = DisplayStyle.None;
            }
        };
        TextBoxWindow.style.display = DisplayStyle.None;
    }

    public void ShowTextBoxWindow(
        Action<string> onTextSubmit = null,
        string initialText = "", 
        bool isCopy = true)
    {
        this.onTextSubmit = onTextSubmit;
        textTextBoxWindow.value = initialText;
        TextBoxWindow.style.display = DisplayStyle.Flex;

        buttonSubmitText.text = isCopy ? "閉じる" : "確定";
        buttonClearText.style.display = Util.Display(!isCopy);
        buttonPasteText.style.display = Util.Display(!isCopy);
        buttonCopyText.style.display = Util.Display(isCopy);
        labelTextBoxWindowTitle.text = isCopy ? 
            "以下のテキストをコピーして保存してください" :
            "セーブデータを以下にペーストしてください";
    }
    #endregion

    #region ProgressWindow
    private void InitializeProgressWindow()
    {
        ProgressWindow.style.display = DisplayStyle.None;
    }

    public async void OnSceneLoadingStart(AsyncOperation op)
    {
        ProgressWindow.style.display = DisplayStyle.Flex;
        progressLoading.value = op.progress;
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            await Awaitable.NextFrameAsync();
            progressLoading.value = op.progress * 100;
        }
        progressLoading.value = 90;
        op.allowSceneActivation = true;
    }
    #endregion

    #region MessageWindow
    private ValueTaskCompletionSource<MessageBoxResult> tcsMessageWindow;
    private void InitializeMessageWindow()
    {
        void OnClick(MessageBoxResult result)
        {
            tcsMessageWindow.SetResult(result);
            tcsMessageWindow = null;
            MessageWindow.style.display = DisplayStyle.None;
        }

        buttonMessageOK.clicked += () => OnClick(MessageBoxResult.Ok);
        buttonMessageYes.clicked += () => OnClick(MessageBoxResult.Yes);
        buttonMessageNo.clicked += () => OnClick(MessageBoxResult.No);
        buttonMessageCancel.clicked += () => OnClick(MessageBoxResult.Cancel);
        MessageWindow.style.display = DisplayStyle.None;
    }

    public ValueTask<MessageBoxResult> ShowMessageWindow(string message, MessageBoxButton button = MessageBoxButton.Ok)
    {
        if (tcsMessageWindow != null) throw new InvalidOperationException();
        tcsMessageWindow = new();

        labelMessageText.text = message;
        buttonMessageOK.style.display = Util.Display(button == MessageBoxButton.Ok);
        buttonMessageYes.style.display = Util.Display(button == MessageBoxButton.YesNo || button == MessageBoxButton.YesNoCancel);
        buttonMessageNo.style.display = Util.Display(button == MessageBoxButton.YesNo || button == MessageBoxButton.YesNoCancel);
        buttonMessageCancel.style.display = Util.Display(button == MessageBoxButton.YesNoCancel);

        MessageWindow.style.display = DisplayStyle.Flex;
        return tcsMessageWindow.Task;
    }
    #endregion
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
