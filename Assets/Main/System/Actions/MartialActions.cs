using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;


public partial class MartialActions
{
    public MartialActions(WorldData world)
    {
        foreach (var action in Actions)
        {
            action.World = world;
        }
    }

    private MartialActionBase[] Actions => GetType()
        .GetProperties()
        .Where(p => p.PropertyType.IsSubclassOf(typeof(MartialActionBase)))
        .Select(p => p.GetValue(this))
        .Cast<MartialActionBase>()
        .ToArray();
}

public class MartialActionBase
{
    public WorldData World { get; set; }

    /// <summary>
    /// 選択肢として表示可能ならtrue
    /// </summary>
    public virtual bool CanSelect(Character chara) => World.IsRuler(chara);
    /// <summary>
    /// アクションの実行に必要なGold
    /// </summary>
    public virtual int Cost(Character chara) => 0;
    /// <summary>
    /// アクションを実行可能ならtrue
    /// </summary>
    public bool CanDo(Character chara) =>
        CanSelect(chara) &&
        chara.Gold >= Cost(chara) &&
        CanDoCore(chara);
    /// <summary>
    /// アクションを実行可能ならtrue（子クラスでのオーバーライド用）
    /// </summary>
    /// <returns></returns>
    protected virtual bool CanDoCore(Character chara) => true;
    /// <summary>
    /// アクションを実行します。
    /// </summary>
    public virtual async Awaitable Do(Character chara) { }
}
