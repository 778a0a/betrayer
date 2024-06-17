using System;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class TitleSceneUI : MonoBehaviour
{
    private void OnEnable()
    {
        InitializeDocument();

        var hasSaveData = SaveData.HasSaveData();
        buttonResumeFromLocalData.SetEnabled(hasSaveData);

        buttonNewGame.clicked += () =>
        {
            MainSceneManager.LoadScene(new MainSceneStartArguments()
            {
                Mode = MainSceneStartMode.NewGame,
            });
        };

        buttonResumeFromLocalData.clicked += () =>
        {
            MainSceneManager.LoadScene(new MainSceneStartArguments()
            {
                Mode = MainSceneStartMode.ResumeFromLocalData,
            });
        };
        
        buttonResumeFromTextData.clicked += () =>
        {
            MainSceneManager.LoadScene(new MainSceneStartArguments()
            {
                Mode = MainSceneStartMode.ResumeFromTextData,
            });
        };

        buttonCloseApplication.clicked += () =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_EDITOR             
            Application.Quit();
#endif
        };
    }
}