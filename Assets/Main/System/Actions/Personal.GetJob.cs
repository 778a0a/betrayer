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
        public override bool CanSelect(Character chara) => World.IsFree(chara);
        public override int Cost(Character chara) => 10;
        protected override bool CanDoCore(Character chara) =>
            World.Countries.Where(c => c.VassalCountMax > c.Vassals.Count).Any();

        public override async Awaitable Do(Character chara)
        {
            Assert.IsTrue(CanSelect(chara));
            Assert.IsTrue(CanDo(chara));
            chara.Gold -= Cost(chara); // TODO

            if (chara.IsPlayer)
            {
                var country = await UI.ShowGetJobScreen(World);
                if (country != null)
                {
                    country.AddVassal(chara);
                }
                UI.ShowIndividualUI();
                UI.IndividualPhase.SetData(chara, World);
            }
            else
            {
                var countries = World.Countries.Where(c => c.VassalCountMax > c.Vassals.Count);
                var country = countries.RandomPick();
                country.AddVassal(chara);
            }

            Core.Tilemap.DrawCountryTile();
        }
    }
}
