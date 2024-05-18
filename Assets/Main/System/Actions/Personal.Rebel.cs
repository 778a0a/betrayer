using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

partial class PersonalActions
{
    /// <summary>
    /// 反乱を起こします。
    /// </summary>
    public RebelAction Rebel { get; } = new();
    public class RebelAction : PersonalActionBase
    {
        public override bool CanSelect(Character chara) => World.IsVassal(chara);
        public override int Cost(Character chara) => 10;
        protected override bool CanDoCore(Character chara) => true;

        public override async Awaitable Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            var country = World.CountryOf(chara);
            var ruler = country.Ruler;

            var target = country.Areas.RandomPick();
            var source = World.Map.GetNeighbors(target).RandomPick();
            var result = await BattleManager.Battle(World.Map, source, target, chara, ruler);
            if (result == BattleResult.AttackerWin)
            {
                var oldRuler = country.Ruler;
                country.Ruler = chara;
                country.Vassals.Remove(chara);
                country.RecalculateSalary();
            }
            else
            {
                country.Vassals.Remove(chara);
                country.RecalculateSalary();
            }

            Core.Tilemap.DrawCountryTile();
        }
    }
}
