using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ActionsBase<TActionBase> where TActionBase : ActionBase
{
    public ActionsBase(GameCore core)
    {
        foreach (var action in Actions)
        {
            action.Core = core;
        }
    }

    protected TActionBase[] Actions => GetType()
        .GetProperties()
        .Where(p => p.PropertyType.IsSubclassOf(typeof(TActionBase)))
        .Select(p => p.GetValue(this))
        .Cast<TActionBase>()
        .ToArray();
}

public class ActionBase
{
    public GameCore Core { get; set; }
    protected WorldData World => Core.World;
    protected MainUI UI => Core.MainUI;
    protected TilemapManager Tilemap => Core.Tilemap; 

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
    public virtual ValueTask Do(Character chara) => new();
}


public partial class MartialActions : ActionsBase<MartialActionBase>
{
    public MartialActions(GameCore core) : base(core)
    {
    }
}
public class MartialActionBase : ActionBase
{
}


public partial class PersonalActions : ActionsBase<PersonalActionBase>
{
    public PersonalActions(GameCore core) : base(core)
    {
    }
}
public class PersonalActionBase : ActionBase
{
}


public partial class StrategyActions : ActionsBase<StrategyActionBase>
{
    public StrategyActions(GameCore core) : base(core)
    {
    }
}
public class StrategyActionBase : ActionBase
{
}
