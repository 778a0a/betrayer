using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

public static class Util
{
    public static TEnum[] EnumArray<TEnum>()
    {
        return (TEnum[])Enum.GetValues(typeof(TEnum));
    }

    public static T RandomPick<T>(this IList<T> list) => list[Random.Range(0, list.Count)];
    public static T RandomPick<T>(this IEnumerable<T> list) => list.ElementAt(Random.Range(0, list.Count()));
    public static T RandomPickDefault<T>(this IList<T> list) => list.Count == 0 ? default : RandomPick(list);
    public static T RandomPickDefault<T>(this IEnumerable<T> list) => list.Count() == 0 ? default : RandomPick(list);

    public static Color Color(string code)
    {
        if (code[0] == '#')
        {
            code = code[1..];
        }
        if (code.Length == 3)
        {
            var r = Convert.ToInt32(code[0].ToString(), 16) / 15f;
            var g = Convert.ToInt32(code[1].ToString(), 16) / 15f;
            var b = Convert.ToInt32(code[2].ToString(), 16) / 15f;
            return new Color(r, g, b);
        }
        else
        {
            var r = Convert.ToInt32(code.Substring(0, 2), 16) / 255f;
            var g = Convert.ToInt32(code.Substring(2, 2), 16) / 255f;
            var b = Convert.ToInt32(code.Substring(4, 2), 16) / 255f;
            return new Color(r, g, b);
        }
    }

    public static Color Color(long val)
    {
        var r = (val >> 16) & 0xFF;
        var g = (val >> 8) & 0xFF;
        var b = val & 0xFF;
        return new Color(r / 255f, g / 255f, b / 255f);
    }
}
