using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CharacterTable
{
    public event EventHandler<Character> RowMouseMove;
    public event EventHandler<Character> RowMouseDown;

    public void Initialize()
    {
        foreach (var row in Rows)
        {
            row.Initialize();
            row.MouseDown += OnRowMouseDown;
            row.MouseMove += OnRowMouseMove;
        }
    }

    private void OnRowMouseMove(object sender, Character e)
    {
        RowMouseMove?.Invoke(this, e);
    }

    private void OnRowMouseDown(object sender, Character e)
    {
        RowMouseDown?.Invoke(this, e);
    }

    private const int RowCount = 10;
    public IEnumerable<CharacterTableRowItem> Rows => Enumerable.Range(0, RowCount).Select(RowOf);
    private CharacterTableRowItem RowOf(int index) => index switch
    {
        0 => row00,
        1 => row01,
        2 => row02,
        3 => row03,
        4 => row04,
        5 => row05,
        6 => row06,
        7 => row07,
        8 => row08,
        9 => row09,
        _ => throw new System.ArgumentOutOfRangeException(),
    };

    public void SetData(Country c)
    {
        var members = c.Members.ToArray();

        for (int i = 0; i < RowCount; i++)
        {
            var row = RowOf(i);
            if (i < members.Length)
            {
                row.SetData(members[i], c);
            }
            else
            {
                row.SetData(null, c);
            }
        }
    }
}