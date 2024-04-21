using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CharacterInfoSoldierIcon
{
    private Color[] levelToColor = new[]
    {
        Util.Color("#FFFFFF"), // (白)
        Util.Color("#FFE0E0"), // (明るいピンク)
        Util.Color("#FFD0D0"), // (ピンク)
        Util.Color("#FFC0C0"), // (濃いピンク)
        Util.Color("#FFFF00"), // (黄色)
        Util.Color("#FF8000"), // (オレンジ)
        Util.Color("#00FF00"), // (緑)
        Util.Color("#00FFFF"), // (シアン)
        Util.Color("#0080FF"), // (青)
        Util.Color("#8000FF"), // (紫)
        Util.Color("#FF0000"), // (赤)
    };

    public void SetData(Soldier s)
    {
        if (s.IsEmptySlot)
        {
            panelContainer.style.backgroundColor = Util.Color("#888");
            labelHp.text = "--";
            return;
        }

        panelContainer.style.backgroundColor = levelToColor[s.Level];
        panelContainer.tooltip = s.ToString();
        labelLevel.text = s.Level.ToString();
        labelHp.text = s.Hp.ToString();
    }
}