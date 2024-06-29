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
    public PrivateFightAction PrivateFight { get; } = new();
    public class PrivateFightAction : MartialActionBase
    {
        public override string Description => "同僚に戦いを仕掛けます。";

        public override bool CanSelect(Character chara) => World.IsVassal(chara);
        public override int Cost(Character chara) => 5;
        protected override bool CanDoCore(Character chara) =>
            !chara.IsAttacked &&
            World.CountryOf(chara).Vassals.Count > 1;

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));

            var country = World.CountryOf(chara);

            var cands = country.Vassals.Where(v => v != chara).ToList();
            var target = cands.RandomPickDefault();
            if (chara.IsPlayer)
            {
                target = await UI.ShowPrivateFightScreen(cands, World);
                if (target == null)
                {
                    Debug.Log("キャンセルされました。");
                    return;
                }
            }

            // 戦う場所を決める。
            var (attack, defend) = country.Areas
                .SelectMany(a => World.Map.GetNeighbors(a)
                    .Where(country.Areas.Contains)
                    .Select(x => (a, x)))
                .DefaultIfEmpty((country.Areas[0], country.Areas[0]))
                .RandomPickDefault();

            // 攻撃する。
            var battle = BattleManager.Prepare(attack, defend, chara, target, this);
            var result = await battle.Do();
            chara.IsAttacked = true;

            // 勝ったら対象を国から排除する。
            if (result == BattleResult.AttackerWin)
            {
                target.Contribution /= 2;
                country.Vassals.Remove(target);
                country.RecalculateSalary();
            }
            // 負けたら自分の貢献を減らす。
            else if (result == BattleResult.DefenderWin)
            {
                chara.Contribution /= 10;
            }

            Core.Tilemap.DrawCountryTile();
            PayCost(chara);
        }
    }
}

