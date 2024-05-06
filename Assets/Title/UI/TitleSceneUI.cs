using System;
using UnityEngine;

public partial class TitleSceneUI : MonoBehaviour
{
    [SerializeField] private SaveDataListWindow saves;
    [SerializeField] private SelectScenarioWindow selectScenario;

    private void OnEnable()
    {
        InitializeDocument();
        saves.Hide();
        selectScenario.Hide();

        StartGame.clicked += StartGame_clicked;
        OpenSeting.clicked += OpenSeting_clicked;
        OpenManual.clicked += OpenManual_clicked;
        FinishApplication.clicked += FinishApplication_clicked;
    }

    private void StartGame_clicked()
    {
        saves.Show();
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