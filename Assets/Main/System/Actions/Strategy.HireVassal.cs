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
        public override int Cost(Character chara) => 8;
        protected override bool CanDoCore(Character chara)
        {
            if (!World.Characters.Any(World.IsFree)) return false;

            // 配下の枠に空きがなければ実行不可。
            var country = World.CountryOf(chara);
            if (country.Vassals.Count >= country.VassalCountMax) return false;

            return true;
        }

        public override async Awaitable Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara);

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

            if (chara.IsPlayer)
            {
                // どのキャラを配下にするか選択する。
                var selected = await Test.Instance.MainUI.ShowSearchResult(candidates.ToArray(), World);
                if (selected == null)
                {
                    Debug.Log("配下にするキャラが選択されませんでした。");
                    Test.Instance.MainUI.ShowStrategyUI();
                    return;
                }
                var country = World.CountryOf(chara);
                country.AddVassal(selected);
                Test.Instance.MainUI.ShowStrategyUI();
            }
            else
            {
                // 配下にする。
                var country = World.CountryOf(chara);
                var newVassal = candidates.OrderByDescending(c => c.Power).First();

                // プレイヤーの場合は選択肢を表示する。
                if (newVassal.IsPlayer)
                {
                    var ok = await Test.Instance.MainUI.ShowRespondJobOfferScreen(country, World);
                    //Test.Instance.MainUI.HideAllUI();
                    if (!ok)
                    {
                        return;
                    }
                }

                country.AddVassal(newVassal);
            }
        }
    }
}
