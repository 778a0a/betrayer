using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CharacterTableRowItem
{
    public event EventHandler<Character> MouseMove;
    public event EventHandler<Character> MouseDown;

    public Character Character { get; private set; }

    public void Initialize()
    {
        Root.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        Root.RegisterCallback<MouseDownEvent>(OnMouseDown);
    }

    private void OnMouseMove(MouseMoveEvent evt)
    {
        MouseMove?.Invoke(this, Character);
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
        MouseDown?.Invoke(this, Character);
    }

    public void SetData(Character chara, Country country)
    {
        Character = chara;
        if (chara == null)
        {
            Root.style.visibility = Visibility.Hidden;
            return;
        }
        Root.style.visibility = Visibility.Visible;
        labelName.text = chara.Name;
        labelAttack.text = chara.Attack.ToString();
        labelDefense.text = chara.Defense.ToString();
        labelIntelligence.text = chara.Intelligence.ToString();
        labelStatus.text = chara == country.Ruler ? "主" : "士";
        labelLoyalty.text = chara.Loyalty.ToString();
        labelPrestige.text = chara.Prestige.ToString();
        labelContribution.text = chara.Contribution.ToString();
        labelSalaryRatio.text = chara.SalaryRatio.ToString();
    }
}