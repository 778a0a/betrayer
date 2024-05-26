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

    public static void SetActionButton(
        Button button,
        ActionBase action,
        Character chara)
    {
        button.style.display = Util.Display(action.CanSelect(chara));
        button.SetEnabled(action.CanDo(chara));
    }
}
