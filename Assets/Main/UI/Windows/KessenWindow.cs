using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public partial class KessenWindow : IWindow
{
    private LocalizationManager L => MainUI.Instance.L;
    
    private KessenMemberAttackSide[] _attackers;
    private KessenMemberDefenceSide[] _defenders;

    public void Initialize()
    {
        Root.style.display = DisplayStyle.None;
        L.Register(this);

        _attackers = new[]
        {
            Attacker00, Attacker01, Attacker02, Attacker03, Attacker04, Attacker05, Attacker06, Attacker07, Attacker08, Attacker09,
        };
        _defenders = new[]
        {
            Defender00, Defender01, Defender02, Defender03, Defender04, Defender05, Defender06, Defender07, Defender08, Defender09,
        };

        foreach (var attacker in _attackers) attacker.Initialize();
        foreach (var defender in _defenders) defender.Initialize();
    }

    public void SetData(Kessen kessen, BattleResult? result = null, bool needInteraction = false)
    {
        var attackers = kessen.Attacker.Members;
        var defenders = kessen.Defender.Members;

        if (result != null)
        {
            buttonAttack.style.display = DisplayStyle.None;
            buttonRetreat.style.display = DisplayStyle.None;
            buttonResult.style.display = DisplayStyle.Flex;
            buttonResult.text = result == BattleResult.AttackerWin ? L["攻撃側の勝利"] : L["防衛側の勝利"];
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
            buttonAttack.style.display = Util.Display(needInteraction);
            buttonRetreat.style.display = Util.Display(needInteraction);
            buttonResult.style.display = DisplayStyle.None;
            Root.RemoveFromClassList("attacker-win");
            Root.RemoveFromClassList("attacker-lose");
            Root.RemoveFromClassList("defender-win");
            Root.RemoveFromClassList("defender-lose");
        }

        AttackerName.text = kessen.Attacker.Country.Ruler.Name;
        DefenderName.text = kessen.Defender.Country.Ruler.Name;

        for (var i = 0; i < _attackers.Length; i++)
        {
            var member = i < attackers.Length ? attackers[i] : null;
            _attackers[i].SetData(member);
        }
        for (var i = 0; i < _defenders.Length; i++)
        {
            var member = i < defenders.Length ? defenders[i] : null;
            _defenders[i].SetData(member);
        }
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