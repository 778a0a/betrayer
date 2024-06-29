using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

partial class CommonActions
{
    /// <summary>
    /// 情報画面を表示します。
    /// </summary>
    public ShowInfoAction ShowInfo { get; } = new();
    public class ShowInfoAction : CommonActionBase
    {
        public override string Description => "マップをクリックして各国の情報を表示します。";

        public override async ValueTask Do(Character chara)
        {
            UI.ShowCountryInfoScreen();
        }
    }
}
