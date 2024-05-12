using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class BattleSoldierIcon
{
    public void SetData(Soldier soldier)
    {
        if (soldier.IsEmptySlot)
        {
            Root.style.visibility = Visibility.Hidden;
            return;
        }
        Root.style.visibility = Visibility.Visible;

        SoldierImage.style.backgroundColor = CharacterInfoSoldierIcon.levelToColor[soldier.Level];
        labelHP.text = soldier.Hp.ToString();
        
        var hpBarLength = new Length(soldier.Hp / (float)soldier.MaxHp * 100, LengthUnit.Percent);
        HPBarValue.style.width = hpBarLength;

        var hpIsLow = soldier.Hp <= 10;
        if (hpIsLow)
        {
            var orange = Util.Color("#FFA500");
            labelHP.style.color = orange;
            HPBarValue.style.backgroundColor = orange;
        }
        else
        {
            labelHP.style.color = Color.white;
            HPBarValue.style.backgroundColor = Color.cyan;
        }
    }
}