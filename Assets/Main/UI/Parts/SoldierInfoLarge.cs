using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SoldierInfoLarge
{
    public static readonly Texture2D EmptyTexture = new(0, 0);

    public void SetData(Soldier s)
    {
        if (s.IsEmptySlot)
        {
            imageSoldier.image = EmptyTexture;
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