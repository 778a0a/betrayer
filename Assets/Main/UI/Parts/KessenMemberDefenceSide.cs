using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class KessenMemberDefenceSide
{
    private IBattleSoldierIcon[] _defenderSoldiers;

    public void Initialize()
    {
        Root.style.visibility = Visibility.Hidden;

        _defenderSoldiers = new[]
        {
            DefenderSoldier00, DefenderSoldier01, DefenderSoldier02, DefenderSoldier03, DefenderSoldier04, DefenderSoldier05, DefenderSoldier06, DefenderSoldier07, DefenderSoldier08, DefenderSoldier09, DefenderSoldier10, DefenderSoldier11, DefenderSoldier12, DefenderSoldier13, DefenderSoldier14,
        };
    }

    public void SetData(CountryInBattle.Member member)
    {
        if (member == null || member.State == KessenMemberState.Retreated)
        {
            Root.style.visibility = Visibility.Hidden;
            for (var i = 0; i < _defenderSoldiers.Length; i++)
            {
                _defenderSoldiers[i].SetData(null);
            }
            return;
        }
        Root.style.visibility = Visibility.Visible;

        var chara = member.Character;
        imageDefender.style.backgroundImage = FaceImageManager.Instance.GetImage(chara);
        for (var i = 0; i < chara.Force.Soldiers.Length; i++)
        {
            var soldier = chara.Force.Soldiers[i];
            _defenderSoldiers[i].SetData(soldier);
        }
    }
}