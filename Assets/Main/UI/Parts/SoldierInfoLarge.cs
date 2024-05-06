using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SoldierInfoLarge
{
    public void SetData(Soldier s)
    {
        if (s.IsEmptySlot)
        {
            imageSoldier.style.backgroundColor = Util.Color("#888");
            labelHp.text = "--";
            return;
        }

        imageSoldier.style.backgroundColor = CharacterInfoSoldierIcon.levelToColor[s.Level];
        imageSoldier.tooltip = s.ToString();
        labelLevel.text = s.Level.ToString();
        labelHp.text = s.Hp.ToString();
    }
}