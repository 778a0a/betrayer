using System;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SaveDataListWindowListItem
{
    public event EventHandler<ButtonType> ButtonClick;
    public enum ButtonType
    {
        Main,
        Download,
        Delete,
        NoData,
    }

    public int SlotNo { get; private set; }
    public bool IsAutoSaveData { get; private set; }
    public SaveDataSummary Summary { get; private set; }

    public void Initialize(int slotNo, bool isAutoSaveData)
    {
        SlotNo = slotNo;
        IsAutoSaveData = isAutoSaveData;
        buttonMain.clicked += () => ButtonClick?.Invoke(this, ButtonType.Main);
        buttonDownload.clicked += () => ButtonClick?.Invoke(this, ButtonType.Download);
        buttonDelete.clicked += () => ButtonClick?.Invoke(this, ButtonType.Delete);
        buttonNoData.clicked += () => ButtonClick?.Invoke(this, ButtonType.NoData);
    }

    public void SetData(SaveDataSummary data)
    {
        Summary = data;
        if (data == null)
        {
            SaveDataLisItemRoot.style.display = DisplayStyle.None;
            buttonNoData.style.display = DisplayStyle.Flex;
            buttonNoData.text = "NEW GAME";
            if (IsAutoSaveData)
            {
                buttonNoData.text = "NO DATA";
                buttonNoData.enabledSelf = false;
            }
            return;
        }
        SaveDataLisItemRoot.style.display = DisplayStyle.Flex;
        buttonNoData.style.display = DisplayStyle.None;

        imageCharacter.image = FaceImageManager.Instance.GetImage(data.FaceImageId);
        labelTitle.text = data.Title;
        labelName.text = data.Name;
        labelSoldiers.text = data.SoldierCount.ToString();
        labelGold.text = data.Gold.ToString();
        labelTurnCount.text = data.TurnCount.ToString();
        labelSavedTime.text = data.SavedTime.ToString();
    }
}
