using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CountryInfo
{
    public void Initialize()
    {
        CharacterTable.Initialize();
        CharacterTable.RowMouseMove += CharacterTable_RowMouseMove; ;
    }

    private void CharacterTable_RowMouseMove(object sender, Character chara)
    {
        if (chara == characterInfoTarget) return;
        characterInfoTarget = chara;
        CharacterInfo.SetData(chara, country);
    }

    private Country country;
    private Area area;
    private Character characterInfoTarget;

    public void ShowCellInformation(WorldData world, MapPosition pos)
    {
        area = world.Map.GetArea(pos);
        country = world.CountryOf(area);
        var ruler = country.Ruler;
        characterInfoTarget = ruler;

        // 地形情報
        labelTerrain.text = area.Terrain.ToString();
        labelPosition.text = pos.ToString();

        // 勢力情報
        imageRuler.image = FaceImageManager.Instance.GetImage(ruler);
        imageCountryColor.sprite = world.Map.Helper.GetCountryImage(country);
        imageCountryColor.style.backgroundColor = new StyleColor(Color.white);
        labelRulerName.text = ruler.Name;
        labelAreaCount.text = country.Areas.Count.ToString();
        labelTotalIncome.text = "xxx";
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

        // 人物情報テーブル
        CharacterTable.SetData(country);
        // 人物詳細
        CharacterInfo.SetData(ruler, country);
    }
}