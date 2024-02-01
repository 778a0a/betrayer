using System;
using UnityEngine;

public partial class TitleSceneUI : MonoBehaviour
{
    private void OnEnable()
    {
        InitializeDocument();

        StartGame.clicked += StartGame_clicked;
        OpenSeting.clicked += OpenSeting_clicked;
        OpenManual.clicked += OpenManual_clicked;
        FinishApplication.clicked += FinishApplication_clicked;
    }

    private void StartGame_clicked()
    {
    }

    private void OpenSeting_clicked()
    {
    }

    private void OpenManual_clicked()
    {
    }

    private void FinishApplication_clicked()
    {
        Application.Quit();
    }
}