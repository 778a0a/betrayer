using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CharacterInfoSoldierIcon
{
    public void SetData(Soldier s)
    {
        if (s.IsEmptySlot)
        {
            imageSoldier.image = null;
            labelHp.text = "--";
            labelLevel.text = "--";
            HPBar.style.visibility = Visibility.Hidden;
            return;
        }
        HPBar.style.visibility = Visibility.Visible;

        //panelContainer.tooltip = s.ToString();
        imageSoldier.image = SoldierImageManager.Instance.GetTexture(s.Level);
        labelLevel.text = s.Level.ToString();
        labelHp.text = s.Hp.ToString();

        HPBarValue.style.width = new Length(s.Hp * 100 / s.MaxHp, LengthUnit.Percent);
    }
}