using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

partial class StrategyActions
{
    /// <summary>
    /// 配下を雇います。
    /// </summary>
    public HireVassalAction HireVassal { get; } = new();
    public class HireVassalAction : StrategyActionBase
    {
        public override string Description => "配下を雇います。";

        public override int Cost(Character chara) => 8;
        protected override bool CanDoCore(Character chara)
        {
            if (!World.Characters.Any(World.IsFree)) return false;

            // 配下の枠に空きがなければ実行不可。
            var country = World.CountryOf(chara);
            if (country.Vassals.Count >= country.VassalCountMax) return false;

            return true;
        }

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            // 探索は成否に拘らずコストを消費する。
            PayCost(chara);

            // ランダムに所属なしのキャラを選ぶ。
            var frees = World.Characters.Where(World.IsFree).ToList();
            var candidates = new List<Character>();
            var candCount = (int)MathF.Max(1, MathF.Ceiling(chara.Intelligence / 10) - 5);
            for (int i = 0; i < candCount; i++)
            {
                if (frees.Count == 0) break;
                var cand = frees.RandomPick();
                candidates.Add(cand);
                frees.Remove(cand);
            }

            // 対象を選ぶ。
            var target = default(Character);
            if (chara.IsPlayer)
            {
                // どのキャラを配下にするか選択する。
                target = await UI.ShowSearchResult(candidates.ToArray(), World);
                if (target == null)
                {
                    Debug.Log("キャンセルされました。");
                    return;
                }
            }
            else
            {
                // 一番強いキャラを選ぶ。
                target = candidates.OrderByDescending(c => c.Power).First();
            }

            // 対象がプレイヤーの場合は選択肢を表示する。
            var country = World.CountryOf(chara);
            if (target.IsPlayer)
            {
                var ok = await UI.ShowRespondJobOfferScreen(country, World);
                //UI.HideAllUI();
                Util.Todo();
                if (!ok)
                {
                    return;
                }
            }
            country.AddVassal(target);

            Core.Tilemap.DrawCountryTile();
        }
    }
}
