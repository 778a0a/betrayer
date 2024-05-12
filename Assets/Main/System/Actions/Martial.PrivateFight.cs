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
    /// 私闘
    /// </summary>
    public static PrivateFightAction PrivateFight { get; } = new();
    public class PrivateFightAction : MartialActionBase
    {
        public override bool CanSelect(Character chara) => World.IsVassal(chara);
        public override int Cost(Character chara) => 5;
        protected override bool CanDoCore(Character chara) =>
            !chara.IsAttacked &&
            World.CountryOf(chara).Vassals.Count > 1;

        public override async Awaitable Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

            var country = World.CountryOf(chara);

            var target = country.Vassals.Where(v => v != chara).RandomPick();

            // 戦う場所を決める。
            var (attack, defend) = country.Areas
                .SelectMany(a => World.Map.GetNeighbors(a)
                    .Where(country.Areas.Contains)
                    .Select(x => (a, x)))
                .DefaultIfEmpty((country.Areas[0], country.Areas[0]))
                .RandomPickDefault();

            // 攻撃する。
            var result = await BattleManager.Battle(World.Map, attack, defend, chara, target);
            chara.IsAttacked = true;
            if (result == BattleResult.AttackerWin)
            {
                BattleManager.Recover(chara, true);
                BattleManager.Recover(target, false);
            }
            else
            {
                BattleManager.Recover(chara, false);
                BattleManager.Recover(target, true);
            }
        }
    }
}

