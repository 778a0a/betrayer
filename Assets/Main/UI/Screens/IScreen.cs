using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public interface IScreen
{
    void Initialize();
    VisualElement Root { get; }
}

public class ActionButtonHelper
{
    private static GameCore Core => GameCore.Instance;
    public static ActionButtonHelper Common(Button b, Func<CommonActions, ActionBase> getter)
        => new(b, () => getter(Core.CommonActions));
    public static ActionButtonHelper Strategy(Button b, Func<StrategyActions, ActionBase> getter)
        => new(b, () => getter(Core.StrategyActions));
    public static ActionButtonHelper Personal(Button b, Func<PersonalActions, ActionBase> getter)
        => new(b, () => getter(Core.PersonalActions));
    public static ActionButtonHelper Martial(Button b, Func<MartialActions, ActionBase> getter)
        => new(b, () => getter(Core.MartialActions));


    public Button Element { get; private set; }
    public ActionBase Action => _Action ??= actionGetter();
    public bool IsMouseOver { get; private set; }

    private Func<ActionBase> actionGetter;
    private ActionBase _Action;
    
    private Label labelCost;
    private Func<Character> currentCharacterGetter;
    private Action<ActionButtonHelper> clickHandler;

    private ActionButtonHelper(Button el, Func<ActionBase> actionGetter)
    {
        Element = el;
        this.actionGetter = actionGetter;
    }

    public void SetEventHandlers(
        Label labelCost,
        Func<Character> currentCharacterGetter,
        Action<ActionButtonHelper> clickHandler)
    {
        this.labelCost = labelCost;
        this.currentCharacterGetter = currentCharacterGetter;
        this.clickHandler = clickHandler;
        Element.RegisterCallback<ClickEvent>(OnActionButtonClicked);
        Element.RegisterCallback<PointerEnterEvent>(OnActionButtonPointerEnter);
        Element.RegisterCallback<PointerLeaveEvent>(OnActionButtonPointerLeave);
    }

    private void OnActionButtonPointerEnter(PointerEnterEvent evt)
    {
        IsMouseOver = true;
        if (Action is CommonActionBase)
        {
            labelCost.text = "---";
        }
        else
        {
            labelCost.text = Action.Cost(currentCharacterGetter()).ToString();
        }
    }

    private void OnActionButtonPointerLeave(PointerLeaveEvent evt)
    {
        IsMouseOver = false;
        labelCost.text = "---";
    }

    private void OnActionButtonClicked(ClickEvent ev)
    {
        clickHandler(this);
    }

    public void SetData(Character chara)
    {
        Element.style.display = Util.Display(Action.CanSelect(chara));
        Element.SetEnabled(Action.CanDo(chara));
        if (IsMouseOver)
        {
            OnActionButtonPointerEnter(null);
        }
    }
}
