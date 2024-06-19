using System;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class TitleSceneUI : MonoBehaviour
{
    private void OnEnable()
    {
        InitializeDocument();

        SaveDataList.Initialize();

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
}