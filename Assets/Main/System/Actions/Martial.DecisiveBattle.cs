using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
        public override string Description => "隣接国と決戦を行います。";

        public override bool CanSelect(Character chara) => World.IsRuler(chara);
        public override int Cost(Character chara) => 10;
        protected override bool CanDoCore(Character chara)
        {
            // 軍事フェイズの先頭の国でないならNG
            if (World.Countries.Any(c => c.IsExhausted)) return false;
            // 決戦禁止期間ならNG
            if (IsCoolingPeriod) return false;
            // 決戦を仕掛けることができる国がないならNG
            if (!Candidates(World.CountryOf(chara)).Any()) return false;
            
            return true;
        }

        /// <summary>
        /// 決戦禁止期間ならtrue
        /// </summary>
        private bool IsCoolingPeriod => Core.TurnCount - Core.LastDecisiveBattleTurnCount < 5;
        /// <summary>
        /// 決戦を仕掛けることができる国のリストを返します。
        /// </summary>
        public List<Country> Candidates(Country c)
        {
            var areas = World.GetAttackableAreas(c);
            return areas.Select(World.CountryOf)
                .Distinct()
                .Where(target => CanBattle(c, target))
                .ToList();
        }

        public static bool CanBattle(Country c, Country target)
        {
            // 対象の国が、自国と同等以上の兵力を持っている。
            return c.Members.Sum(m => m.Force.SoldierCount) * 0.80f
                <=
                target.Members.Sum(m => m.Force.SoldierCount);
        }

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            
            var country = World.CountryOf(chara);
            var cands = Candidates(country);
            var target = default(Country);
            if (chara.IsPlayer)
            {
                target = await UI.ShowSelectDecisiveBattleTargetScreen(country, cands, World);
                if (target == null)
                {
                    Debug.Log("決戦を中止しました。");
                    return;
                }
            }
            else
            {
                target = cands.OrderBy(c => c.Members.Sum(m => m.Power)).First();
            }

            await MessageWindow.Show($"{country.Name}が{target.Name}に決戦を仕掛けました。");

            var kessen = Kessen.Prepare(country, target);
            var result = await kessen.Do();

            // 攻撃側の勝ち
            if (result == BattleResult.AttackerWin)
            {
                foreach (var m in kessen.Attacker.Members) m.Character.Contribution += 30;
                foreach (var m in kessen.Defender.Members) m.Character.Contribution += 5;
            }
            // 防衛側の勝ち
            else
            {
                foreach (var m in kessen.Attacker.Members) m.Character.Contribution += 5;
                foreach (var m in kessen.Defender.Members) m.Character.Contribution += 30;
            }

            if (country.Vassals.Any(m => m.IsPlayer) || target.Members.Any(m => m.IsPlayer))
            {
                UI.MartialPhase.SetData(chara, World);
                UI.ShowMartialUI();
                Util.Todo();
            }

            var attackableArea = World.GetAttackableAreas(target).First();
            Core.MartialActions.Attack.contexts[chara] = (target, attackableArea);

            Core.LastDecisiveBattleTurnCount = Core.TurnCount;
            PayCost(chara);
        }
    }
}

