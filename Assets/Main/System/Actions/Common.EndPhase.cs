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
    /// 自分のフェイズを終了します。
    /// </summary>
    public EndPhaseAction EndPhase { get; } = new();
    public class EndPhaseAction : CommonActionBase
    {
        public override string Description => L["次のフェイズに進みます。"];

        public override async ValueTask Do(Character chara)
        {
            Test.Instance.hold = false;
        }
    }
}
