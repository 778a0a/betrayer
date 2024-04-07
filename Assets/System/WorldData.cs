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
    public List<Country> Countries { get; set; }
    public MapGrid Map { get; set; }

    public bool IsRuler(Character chara) => Countries.Any(c => c.Ruler == chara);
    public bool IsVassal(Character chara) => Countries.Any(c => c.Vassals.Contains(chara));
    public bool IsFree(Character chara) => !IsRuler(chara) && !IsVassal(chara);

    public Country CountryOf(Character chara) => Countries.FirstOrDefault(c => c.Ruler == chara || c.Vassals.Contains(chara));
    public Country CountryOf(Area area) => Countries.FirstOrDefault(c => c.Areas.Contains(area));

    public override string ToString() => $"WorldData {Characters.Length} characters, {Countries.Count} countries";
}

public enum BattleResult
{
    None = 0,
    AttackerWin,
    DefenderWin,
    Draw,
}