using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class BattleDialog : IDialog
{
    private BattleSoldierIcon[] _attackerSoldiers;
    private BattleSoldierIcon[] _defenderSoldiers;

    public void Initialize()
    {
        _attackerSoldiers = new[]
        {
            AttackerSoldier00, AttackerSoldier01, AttackerSoldier02, AttackerSoldier03, AttackerSoldier04, AttackerSoldier05, AttackerSoldier06, AttackerSoldier07, AttackerSoldier08, AttackerSoldier09, AttackerSoldier10, AttackerSoldier11, AttackerSoldier12, AttackerSoldier13, AttackerSoldier14,
        };
        _defenderSoldiers = new[]
        {
            DefenderSoldier00, DefenderSoldier01, DefenderSoldier02, DefenderSoldier03, DefenderSoldier04, DefenderSoldier05, DefenderSoldier06, DefenderSoldier07, DefenderSoldier08, DefenderSoldier09, DefenderSoldier10, DefenderSoldier11, DefenderSoldier12, DefenderSoldier13, DefenderSoldier14,
        };
    }

    public void SetData(
        Character attacker,
        Terrain attackerTerrain,
        Character defender,
        Terrain defenderTerrain)
    {
        AttackerName.text = attacker.Name;
        DefenderName.text = defender.Name;

        for (var i = 0; i < _attackerSoldiers.Length; i++)
        {
            var soldier = attacker.Force.Soldiers[i];
            _attackerSoldiers[i].SetData(soldier);
        }
        for (var i = 0; i < _defenderSoldiers.Length; i++)
        {
            var soldier = defender.Force.Soldiers[i];
            _defenderSoldiers[i].SetData(soldier);
        }

        imageAttacker.style.backgroundImage = FaceImageManager.Instance.GetImage(attacker);
        labelAttackerAttack.text = attacker.Attack.ToString();
        labelAttackerIntelligense.text = attacker.Intelligence.ToString();
        labelAttackerTerrain.text = attackerTerrain.ToString();

        imageDefender.style.backgroundImage = FaceImageManager.Instance.GetImage(defender);
        labelDefenderDefence.text = defender.Defense.ToString();
        labelDefenderIntelligense.text = defender.Intelligence.ToString();
        labelDefenderTerrain.text = defenderTerrain.ToString();
    }
}