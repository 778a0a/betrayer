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
    /// 既存勢力に仕官します。
    /// </summary>
    public GetJobAction GetJob { get; } = new();
    public class GetJobAction : PersonalActionBase
    {
        public override string Description => L["既存勢力に仕官します。"];

        public override bool CanSelect(Character chara) => World.IsFree(chara);
        public override int Cost(Character chara) => 10;
        protected override bool CanDoCore(Character chara) => chara.IsPlayer ? 
            // プレーヤーなら最大配下数を超えて仕官できるようにする。(ただし上限は超えないようにする)
            World.Countries.Where(c => Country.VassalCountMaxLimit > c.Vassals.Count).Any() :
            World.Countries.Where(c => c.VassalCountMax > c.Vassals.Count).Any();

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));

            var country = World.CountryOf(chara);
            if (chara.IsPlayer)
            {
                country = await UI.ShowGetJobScreen(World);
                if (country == null)
                {
                    Debug.Log("キャンセルされました。");
                    return;
                }

            }
            else
            {
                var countries = World.Countries.Where(c => c.VassalCountMax > c.Vassals.Count);
                country = countries.RandomPick();
            }

            if (chara.IsPlayer)
            {
                if (country.Ruler.Urami > 0)
                {
                    await MessageWindow.Show(L["拒否されました。"]);
                    PayCost(chara);
                    return;
                }
            }

            country.AddVassal(chara);

            if (chara.IsPlayer)
            {
                await MessageWindow.Show(L["{0}に仕官しました。", country.Name]);
            }

            Core.Tilemap.DrawCountryTile();
            PayCost(chara);
        }
    }
}
