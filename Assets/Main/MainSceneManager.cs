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

        switch (s_args.Mode)
        {
            case MainSceneStartMode.NewGame:
                test.StartNewGame();
                break;
            case MainSceneStartMode.ResumeFromLocalData:
                {
                    var saveData = SaveData.LoadSaveData();
                    test.ResumeGame(saveData);
                    break;
                }
            case MainSceneStartMode.ResumeFromTextData:
                {
                    var saveData = GUIUtility.systemCopyBuffer;
                    test.ResumeGame(saveData);
                    break;
                }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public class MainSceneStartArguments
{
    public MainSceneStartMode Mode { get; set; }
}

public enum MainSceneStartMode
{
    NewGame,
    ResumeFromLocalData,
    ResumeFromTextData
}
