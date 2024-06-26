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
