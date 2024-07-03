using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Test : MonoBehaviour
{
    public static Test Instance { get; private set; }

    [SerializeField] private TilemapData initialTilemapData;
    [SerializeField] public float wait = 1;
    [SerializeField] private MainUI MainUI;
    [SerializeField] public TilemapManager tilemap;
    [SerializeField] private Texture2D soldierTexture;

    public bool showOthersBattle = true;

    private GameCore core;

    private void Awake()
    {
        Instance = this;

        FaceImageManager.Instance.ClearCache();
        SoldierImageManager.Instance.Initialize(soldierTexture);
    }

    private void CreateWorldOld()
    {
        //world = DefaultData.InitializeDefaultData(initialTilemapData, tilemapHelper);
        //SaveData.SaveWorldData(world);
        //return;
    }

    public void StartNewGame(int saveDataSlotNo)
    {
        var world = SaveData.LoadDefaultWorldData(tilemap.Helper);
        StartGame(world, null, saveDataSlotNo, true);
    }

    public void ResumeGame(WorldAndState ws, int saveDataSlotNo)
    {
        StartGame(ws.World, ws.State, saveDataSlotNo, false);
    }

    private void StartGame(WorldData world, SavedGameCoreState state, int saveDataSlotNo, bool isNewGame)
    {
        core = new GameCore(world, MainUI, tilemap, this, state, saveDataSlotNo);
        GameCore.Instance = core;
        tilemap.DrawCountryTile();

        MainUI.MainUIButtonClick += MainUI_MainUIButtonClick;
        MainUI.StrategyPhase.ActionButtonClicked += StrategyPhaseScreen_ActionButtonClicked;
        MainUI.IndividualPhase.ActionButtonClicked += IndividualPhaseScreen_ActionButtonClicked;
        MainUI.MartialPhase.ActionButtonClicked += MartialPhaseScreen_ActionButtonClicked;

        hold = false;
        holdOnTurnEnd = false;
        setHoldOnHoldEnd = false;

        if (isNewGame)
        {
            // 操作キャラを選択してもらう。
            // 初期表示はランダムにする。
            MainUI.ShowSelectPlayerCharacterUI(world, chara =>
            {
                // 観戦モード
                if (chara == null)
                {
                    Debug.Log("観戦モードが選択されました。");
                    core.IsWatchMode = true;
                    core.MainUI.WatchModeWindow.Show();
                }
                else
                {
                    chara.IsPlayer = true;
                    Debug.Log($"Player selected: {chara.Name}");
                }
                core.DoMainLoop().Foreget();
            });
            MessageWindow.Show("操作キャラを選択してください。");
        }
        else
        {
            core.DoMainLoop().Foreget();
        }
    }

    private async void StrategyPhaseScreen_ActionButtonClicked(object sender, ActionButtonHelper button)
    {
        var chara = MainUI.StrategyPhase.debugCurrentChara;
        var action = button.Action;
        
        if (action is CommonActionBase)
        {
            if (action.CanDo(chara))
            {
                await action.Do(chara);
            }
            return;
        }
        
        if (action.CanDo(chara))
        {
            await action.Do(chara);
            OnActionEnd();
        }

        void OnActionEnd()
        {
            MainUI.StrategyPhase.SetData(chara, core.World);
            MainUI.ShowStrategyUI();
        }
    }

    private async void IndividualPhaseScreen_ActionButtonClicked(object sender, ActionButtonHelper button)
    {
        var chara = MainUI.IndividualPhase.debugCurrentChara;
        var action = button.Action;

        if (action is CommonActionBase)
        {
            if (action.CanDo(chara))
            {
                await action.Do(chara);
            }
            return;
        }

        if (action.CanDo(chara))
        {
            await action.Do(chara);
            OnActionEnd();
        }

        void OnActionEnd()
        {
            MainUI.IndividualPhase.SetData(chara, core.World);
            MainUI.ShowIndividualUI();
        }
    }

    private async void MartialPhaseScreen_ActionButtonClicked(object sender, ActionButtonHelper button)
    {
        var chara = MainUI.MartialPhase.debugCurrentChara;
        var action = button.Action;

        if (action is CommonActionBase)
        {
            if (action.CanDo(chara))
            {
                await action.Do(chara);
            }
            return;
        }

        if (action.CanDo(chara))
        {
            await action.Do(chara);
            OnActionEnd();
        }

        void OnActionEnd()
        {
            MainUI.MartialPhase.SetData(chara, core.World);
            MainUI.ShowMartialUI();
        }
    }

    private void MainUI_MainUIButtonClick(object sender, MainUI.MainUIButton e)
    {
        switch (e)
        {
            case MainUI.MainUIButton.ToggleDebugUI:
                MainUI.DebugUI.visible = !MainUI.DebugUI.visible;
                break;
            case MainUI.MainUIButton.NextPhase:
                hold = false;
                break;
            case MainUI.MainUIButton.NextTurn:
                hold = false;
                setHoldOnHoldEnd = false;
                holdOnTurnEnd = true;
                break;
            case MainUI.MainUIButton.Auto:
                hold = false;
                setHoldOnHoldEnd = false;
                holdOnTurnEnd = false;
                break;
            case MainUI.MainUIButton.Hold:
                hold = true;
                break;
        }
    }

    public void OnEnterStrategyPhase()
    {
        MainUI.ShowStrategyUI();
    }

    public void OnTickStrategyPhase(Character chara)
    {
        MainUI.StrategyPhase.SetData(chara, core.World);
    }

    public void OnEnterIndividualPhase()
    {
        MainUI.ShowIndividualUI();
    }

    public void OnTickIndividualPhase(Character chara)
    {
        MainUI.IndividualPhase.SetData(chara, core.World);
        //yield return new WaitForSeconds(0.05f);
    }

    public void OnEnterMartialPhase()
    {
        MainUI.ShowMartialUI();
    }

    public void OnTickMartialPhase(Character chara)
    {
        MainUI.MartialPhase.SetData(chara, core.World);
    }

    public bool holdOnTurnEnd = false;
    private bool setHoldOnHoldEnd = false;
    public bool hold = false;
    public async ValueTask HoldIfNeeded()
    {
        while (hold)
        {
            if (MainUI.WatchModeWindow.IsVisible)
            {
                MainUI.WatchModeWindow.Hide();
            }
            await Awaitable.WaitForSecondsAsync(0.1f);
        }
        hold = setHoldOnHoldEnd;
    }

    public async ValueTask WaitUserInteraction()
    {
        hold = true;
        await HoldIfNeeded();
    }
}
