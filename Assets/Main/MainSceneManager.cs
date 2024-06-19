using System;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{
    private static MainSceneStartArguments s_args;
    public static void LoadScene(MainSceneStartArguments args)
    {
        s_args = args;
        SceneManager.LoadScene("MainScene");
    }

    private Test test;

    void Start()
    {
        TryGetComponent(out test);

        var args = s_args ?? new MainSceneStartArguments()
        {
            IsNewGame = true,
            SaveDataSlotNo = 0,
        };

        if (args.IsNewGame)
        {
            test.StartNewGame(args.SaveDataSlotNo);
            return;
        }

        var saveData = SaveDataManager.Instance.Load(args.SaveDataSlotNo, test.tilemap.Helper);
        test.ResumeGame(saveData);
    }
}

public class MainSceneStartArguments
{
    public bool IsNewGame { get; set; }
    public int SaveDataSlotNo { get; set; }
    public SaveDataSummary Summary { get; set; }
}

public enum MainSceneStartMode
{
    NewGame,
    ResumeFromLocalData,
    ResumeFromTextData
}
