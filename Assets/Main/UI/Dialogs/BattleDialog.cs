using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

    public void SetData(Battle battle, BattleResult? result = null)
    {
        var attacker = battle.Attacker.Character;
        var defender = battle.Defender.Character;
        var attackerTerrain = battle.Attacker.Terrain;
        var defenderTerrain = battle.Defender.Terrain;

        if (result != null)
        {
            buttonAttack.style.display = DisplayStyle.None;
            buttonRetreat.style.display = DisplayStyle.None;
            buttonResult.style.display = DisplayStyle.Flex;
            buttonResult.text = result == BattleResult.AttackerWin ? "攻撃側の勝利" : "防衛側の勝利";
            if (result == BattleResult.AttackerWin)
            {
                Root.AddToClassList("attacker-win");
                Root.AddToClassList("defender-lose");
            }
            else
            {
                Root.AddToClassList("attacker-lose");
                Root.AddToClassList("defender-win");
            }
        }
        else
        {
            buttonAttack.style.display = Util.Display(battle.NeedInteraction);
            buttonRetreat.style.display = Util.Display(battle.NeedInteraction);
            buttonResult.style.display = DisplayStyle.None;
            Root.RemoveFromClassList("attacker-win");
            Root.RemoveFromClassList("attacker-lose");
            Root.RemoveFromClassList("defender-win");
            Root.RemoveFromClassList("defender-lose");
        }

        AttackerName.text = attacker.Name;
        DefenderName.text = defender.Name;

        for (var i = 0; i < attacker.Force.Soldiers.Length; i++)
        {
            var soldier = attacker.Force.Soldiers[i];
            _attackerSoldiers[i].SetData(soldier);
        }
        for (var i = 0; i < defender.Force.Soldiers.Length; i++)
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

    public ValueTask<bool> WaitPlayerClick()
    {
        var tcs = new ValueTaskCompletionSource<bool>();

        buttonAttack.clicked += buttonAttackClicked;
        void buttonAttackClicked()
        {
            tcs.SetResult(true);
            buttonAttack.clicked -= buttonAttackClicked;
            buttonRetreat.clicked -= buttonRetreatClicked;
            buttonResult.clicked -= buttonResultClicked;
        }

        buttonRetreat.clicked += buttonRetreatClicked;
        void buttonRetreatClicked()
        {
            tcs.SetResult(false);
            buttonAttack.clicked -= buttonAttackClicked;
            buttonRetreat.clicked -= buttonRetreatClicked;
            buttonResult.clicked -= buttonResultClicked;
        }

        buttonResult.clicked += buttonResultClicked;
        void buttonResultClicked()
        {
            tcs.SetResult(true);
            buttonAttack.clicked -= buttonAttackClicked;
            buttonRetreat.clicked -= buttonRetreatClicked;
            buttonResult.clicked -= buttonResultClicked;
        }

        return tcs.Task;
    }
}