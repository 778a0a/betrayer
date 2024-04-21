using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEngine.Rendering.DebugUI.Table;

public partial class CharacterInfo
{
    public Character Character { get; private set; }

    private const int SoldierIconCount = 10;
    private CharacterInfoSoldierIcon SoldierIconOf(int index) => index switch
    {
        0 => soldier00,
        1 => soldier01,
        2 => soldier02,
        3 => soldier03,
        4 => soldier04,
        5 => soldier05,
        6 => soldier06,
        7 => soldier07,
        8 => soldier08,
        9 => soldier09,
        _ => throw new System.ArgumentOutOfRangeException(),
    };

    public void SetData(Character chara, Country country)
    {
        Character = chara;
        labelCharaName.text = chara.Name;
        labelCharaName.text = chara.Name;
        labelCharaAttack.text = chara.Attack.ToString();
        labelCharaDefense.text = chara.Defense.ToString();
        labelCharaIntelligence.text = chara.Intelligence.ToString();
        labelCharaStatus.text = chara == country.Ruler ? "主" : "士";
        labelCharaLoyalty.text = chara.Loyalty.ToString();
        labelCharaPrestige.text = chara.Prestige.ToString();
        labelCharaContribution.text = chara.Contribution.ToString();
        labelCharaSalaryRatio.text = chara.SalaryRatio.ToString();
        imageChara.image = FaceImageManager.Instance.GetImage(chara);

        for (int i = 0; i < SoldierIconCount; i++)
        {
            var sol = chara.Force.Soldiers[i];
            var icon = SoldierIconOf(i);
            icon.SetData(sol);
        }
    }
}