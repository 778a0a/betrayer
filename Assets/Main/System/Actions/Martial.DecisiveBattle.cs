using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;


partial class MartialActions
{
    /// <summary>
    /// 決戦
    /// </summary>
    public DecisiveBattleAction DecisiveBattle { get; } = new();
    public class DecisiveBattleAction : MartialActionBase
    {
        public override bool CanSelect(Character chara) => World.IsRuler(chara);
        public override int Cost(Character chara) => 10;
        protected override bool CanDoCore(Character chara) => true; // TODO
            

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));

            PayCost(chara);
        }
    }
}

