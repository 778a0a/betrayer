using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Assertions;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

public partial class PersonalActions
{
    public static void Initialize(WorldData world)
    {
        var actions = typeof(PersonalActions).GetProperties()
            .Where(p => p.PropertyType.IsSubclassOf(typeof(PersonalActionBase)))
            .Select(p => p.GetValue(null))
            .Cast<PersonalActionBase>()
            .ToList();

        foreach (var action in actions)
        {
            action.World = world;
        }
    }
}

public class PersonalActionBase
{
    public WorldData World { get; set; }

    /// <summary>
    /// 選択肢として表示可能ならtrue
    /// </summary>
    public virtual bool CanSelect(Character chara) => true;
    /// <summary>
    /// アクションの実行に必要なGold
    /// </summary>
    public virtual int Cost(Character chara) => 0;
    /// <summary>
    /// アクションを実行可能ならtrue
    /// </summary>
    public bool CanDo(Character chara) => CanSelect(chara) && chara.Gold >= Cost(chara) && CanDoCore(chara);
    /// <summary>
    /// アクションを実行可能ならtrue（子クラスでのオーバーライド用）
    /// </summary>
    /// <returns></returns>
    protected virtual bool CanDoCore(Character chara) => true;
    /// <summary>
    /// アクションを実行します。
    /// </summary>
    public virtual void Do(Character chara) { }
}
