using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SoldierInfoLarge
{

    public void SetData(Soldier s)
    {
        if (s.IsEmptySlot)
        {
            imageSoldier.image = SoldierImageManager.Instance.GetEmptyTexture();
            labelLevel.text = "--";
            labelHp.text = "--";
            return;
        }

        imageSoldier.image = SoldierImageManager.Instance.GetTexture(s.Level);
        imageSoldier.tooltip = s.ToString();
        labelLevel.text = s.Level.ToString();
        labelHp.text = s.Hp.ToString();
    }
}