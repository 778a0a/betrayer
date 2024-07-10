using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{
    private static MainSceneStartArguments s_args;
    public static AsyncOperation LoadScene(MainSceneStartArguments args)
    {
        s_args = args;
        return SceneManager.LoadSceneAsync("MainScene");
    }

    private Test test;

    void Start()
    {
        TryGetComponent(out test);

        var args = s_args ?? new MainSceneStartArguments()
        {
            IsNewGame = true,
            NewGameSaveDataSlotNo = 0,
        };

        if (args.IsNewGame)
        {
            test.StartNewGame(args.NewGameSaveDataSlotNo);
            return;
        }

        var saveData = args.SaveData;
        var worldData = saveData.RestoreWorldData(test.tilemap.Helper);
        var ws = new WorldAndState(worldData, saveData.State);
        test.ResumeGame(ws, saveData.Summary.SaveDataSlotNo);
    }

    private void Update()
    {
        // Ctrl+Alt+Shift+Tでタイトルに戻る。
        var ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        var alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        var shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (ctrl && shift && alt && Input.GetKeyDown(KeyCode.T))
        {
            TitleSceneManager.LoadScene();
        }
    }
}

public class MainSceneStartArguments
{
    public bool IsNewGame { get; set; }
    public int NewGameSaveDataSlotNo { get; set; }
    public SaveData SaveData { get; set; }
}

public enum MainSceneStartMode
{
    NewGame,
    ResumeFromLocalData,
    ResumeFromTextData
}
