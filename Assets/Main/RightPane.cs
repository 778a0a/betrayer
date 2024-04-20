using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class RightPane : MonoBehaviour
{

    private void OnEnable()
    {
        InitializeDocument();
    }

    public void ShowCellInformation(WorldData world, MapPosition pos)
    {
        var area = world.Map.GetArea(pos);
        var country = world.CountryOf(area);
        var ruler = country.Ruler;


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

        // 人物情報
    }
}

public class FaceImageManager
{
    public static FaceImageManager Instance { get; } = new();

    private Dictionary<string, Texture2D> cacheImages = new();
    public void ClearCache() => cacheImages.Clear();

    public Texture2D GetImage(Character chara)
    {
        var path = chara.debugImagePath;
        if (path == null) return null;

        if (cacheImages.TryGetValue(path, out var tex))
        {
            return tex;
        }

        var bytes = System.IO.File.ReadAllBytes(path);
        tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);
        cacheImages[path] = tex;
        return tex;
    }

}