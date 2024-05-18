using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

public class WorldData
{
    public Character[] Characters { get; set; }
    // なぜかList<T>だと、HotReload後にforeachしたときにエラーが起きるのでIList<T>を使います。
    public IList<Country> Countries { get; set; }
    public MapGrid Map { get; set; }

    public bool IsRuler(Character chara) => Countries.Any(c => c.Ruler == chara);
    public bool IsVassal(Character chara) => Countries.Any(c => c.Vassals.Contains(chara));
    public bool IsFree(Character chara) => !IsRuler(chara) && !IsVassal(chara);
    public bool IsRulerOrVassal(Character chara) => IsRuler(chara) || IsVassal(chara);

    public Country CountryOf(Character chara) => Countries.FirstOrDefault(c => c.Ruler == chara || c.Vassals.Contains(chara));
    public Country CountryOf(Area area) => Countries.FirstOrDefault(c => c.Areas.Contains(area));
    public Country[] Neighbors(Country country) =>
        country.Areas.SelectMany(a => Map.GetNeighbors(a))
        .Select(CountryOf)
        .Distinct()
        .Except(new[] { country })
        .ToArray();

    /// <summary>
    /// ある国の攻撃可能なエリアを取得します。
    /// </summary>
    public List<Area> GetAttackableAreas(Country country)
    {
        var neighborAreas = new List<Area>();
        foreach (var area in country.Areas)
        {
            var neighbors = Map.GetNeighbors(area);
            foreach (var neighbor in neighbors)
            {
                // 自国か同盟国ならスキップする。
                var owner = CountryOf(neighbor);
                if (owner == country || owner == country.Ally) continue;
                neighborAreas.Add(neighbor);
            }
        }

        return neighborAreas;
    }


    public override string ToString() => $"WorldData {Characters.Length} characters, {Countries.Count} countries";
}

public enum BattleResult
{
    None = 0,
    AttackerWin,
    DefenderWin,
    Draw,
}