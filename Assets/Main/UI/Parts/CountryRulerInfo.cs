using System;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CountryRulerInfo
{
    public void SetData(Country country, WorldData world)
    {
        if (country == null)
        {
            Root.style.visibility = Visibility.Hidden;
            return;
        }
        Root.style.visibility = Visibility.Visible;

        imageRuler.image = FaceImageManager.Instance.GetImage(country.Ruler);
        imageCountryColor.sprite = world.Map.Helper.GetCountryImage(country);
        imageCountryColor.style.backgroundColor = new StyleColor(Color.white);
        labelRulerName.text = country.Ruler.Name;
        labelAreaCount.text = country.Areas.Count.ToString();
        labelTotalIncome.text = country.TotalIncome.ToString();
        labelTotalGold.text = country.Members.Select(m => m.Gold).Sum().ToString();
        labelMemberCount.text = country.Members.Count().ToString();
        labelSoldierCount.text = country.Members.SelectMany(m => m.Force.Soldiers.Select(s => s.Hp)).Sum().ToString();
        labelTotalPower.text = country.Members.Select(m => m.Power).Sum().ToString();
        if (country.Ally != null)
        {
            labelAlly.text = country.Ally.Ruler.Name;
            imageAllyCountryColor.sprite = world.Map.Helper.GetCountryImage(country.Ally);
        }
        else
        {
            labelAlly.text = "なし";
            imageAllyCountryColor.sprite = null;
            imageAllyCountryColor.style.visibility = Visibility.Hidden;
        }
    }
}