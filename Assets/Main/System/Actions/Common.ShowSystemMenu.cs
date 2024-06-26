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
    /// システムメニューを表示します。
    /// </summary>
    public ShowSystemMenuAction ShowSystemMenu { get; } = new();
    public class ShowSystemMenuAction : CommonActionBase
    {
        public override async ValueTask Do(Character chara)
        {
            UI.SystemWindow.Show();
        }
    }
}
