using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class KessenMemberAttackSide
{
    private IBattleSoldierIcon[] _attackerSoldiers;

    public void Initialize()
    {
        Root.style.visibility = Visibility.Hidden;

        _attackerSoldiers = new[]
        {
            AttackerSoldier00, AttackerSoldier01, AttackerSoldier02, AttackerSoldier03, AttackerSoldier04, AttackerSoldier05, AttackerSoldier06, AttackerSoldier07, AttackerSoldier08, AttackerSoldier09, AttackerSoldier10, AttackerSoldier11, AttackerSoldier12, AttackerSoldier13, AttackerSoldier14,
        };
    }

    public void SetData(CountryInBattle.Member member)
    {
        if (member == null || member.State == KessenMemberState.Retreated)
        {
            Root.style.visibility = Visibility.Hidden;
            for (var i = 0; i < _attackerSoldiers.Length; i++)
            {
                _attackerSoldiers[i].SetData(null);
            }
            return;
        }
        Root.style.visibility = Visibility.Visible;

        var chara = member.Character;
        imageAttacker.style.backgroundImage = FaceImageManager.Instance.GetImage(chara);
        for (var i = 0; i < chara.Force.Soldiers.Length; i++)
        {
            var soldier = chara.Force.Soldiers[i];
            _attackerSoldiers[i].SetData(soldier);
        }
    }
}