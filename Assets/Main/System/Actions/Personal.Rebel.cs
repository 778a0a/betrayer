﻿using System;
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
        public override string Description => "反乱を起こします。";

        public override bool CanSelect(Character chara) => World.IsVassal(chara);
        public override int Cost(Character chara) => 10;
        protected override bool CanDoCore(Character chara) => true;

        public override async ValueTask Do(Character chara)
        {
            Assert.IsTrue(CanDo(chara));

            if (chara.IsPlayer)
            {
                var res = await MessageWindow.Show("本当に反乱を起こしますか？", MessageBoxButton.OkCancel);
                if (res != MessageBoxResult.Ok) return;
            }

            var country = World.CountryOf(chara);
            var ruler = country.Ruler;

            var target = country.Areas.RandomPick();
            var source = World.Map.GetNeighbors(target).RandomPick();
            
            var battle = BattleManager.Prepare(source, target, chara, ruler, this);
            var result = await battle.Do();

            // 勝ったら君主に成り上がる。
            if (result == BattleResult.AttackerWin)
            {
                var oldRuler = country.Ruler;
                country.Ruler = chara;
                country.Vassals.Remove(chara);
                country.RecalculateSalary();
                if (chara.IsPlayer)
                {
                    await MessageWindow.Show("反乱成功！\n新しい君主になりました。");
                }
            }
            // 負けたら未所属になる。
            else
            {
                country.Vassals.Remove(chara);
                country.RecalculateSalary();
                if (chara.IsPlayer)
                {
                    await MessageWindow.Show("反乱は失敗し、勢力を追放されました。");
                }
            }

            Core.Tilemap.DrawCountryTile();
            PayCost(chara);
        }
    }
}
