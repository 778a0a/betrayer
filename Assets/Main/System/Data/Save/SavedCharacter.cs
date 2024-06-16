﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;

public static class SavedCharacters
{
    public static string Serialize(WorldData world)
    {
        var charas = new List<SavedCharacter>();
        for (int i = 0; i < world.Characters.Length; i++)
        {
            var character = world.Characters[i];
            var country = world.Countries.FirstOrDefault(c => c.Members.Contains(character));
            var memberIndex = country?.Members.TakeWhile(c => c != character).Count() ?? -1;
            var chara = new SavedCharacter
            {
                Character = character,
                CountryId = country != null ? country.Id : -1,
                MemberOrderIndex = memberIndex,
            };
            charas.Add(chara);
        }
        charas = charas.OrderBy(c => c.CountryId).ThenBy(c => c.MemberOrderIndex).ToList();

        var csv = SavedCharacter.CreateCsv(charas);
        return csv;
    }

    public static List<SavedCharacter> Deserialize(string csv)
    {
        var lines = csv.Trim().Split('\n');
        var header = lines[0].Trim().Split('\t');
        var charas = new List<SavedCharacter>();
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }
            var chara = SavedCharacter.Deserialize(header, line);
            charas.Add(chara);
        }
        return charas;
    }
}

public class SavedCharacter
{
    public int CountryId { get; set; }
    public int MemberOrderIndex { get; set; }
    public Character Character { get; set; }

    public bool IsRuler => !IsFree && MemberOrderIndex == 0;
    public bool IsFree => CountryId == -1;

    private static readonly string EmptySlotMark = "E";

    public static string CreateCsv(List<SavedCharacter> charas)
    {
        var json = JsonConvert.SerializeObject(charas);
        var list = JsonConvert.DeserializeObject<List<JObject>>(json);
        var sb = new StringBuilder();

        bool IsTargetProperty(JProperty prop)
        {
            return
                !prop.Name.Equals(nameof(global::Character.debugMemo)) &&
                !prop.Name.Equals(nameof(global::Character.debugImagePath));
        }

        var delimiter = "\t";
        // ヘッダー
        sb.Append(nameof(CountryId)).Append(delimiter);
        sb.Append(nameof(MemberOrderIndex)).Append(delimiter);
        foreach (JProperty prop in list[0][nameof(Character)])
        {
            if (!IsTargetProperty(prop)) continue;
            sb.Append(prop.Name).Append(delimiter);
        }
        sb.AppendLine();

        // 中身
        for (var i = 0; i < charas.Count; i++)
        {
            var chara = charas[i].Character;
            var obj = list[i];

            sb.Append(obj[nameof(CountryId)]).Append(delimiter);
            sb.Append(obj[nameof(MemberOrderIndex)]).Append(delimiter);
            foreach (JProperty prop in obj[nameof(Character)])
            {
                if (!IsTargetProperty(prop)) continue;

                if (prop.Name.Equals(nameof(global::Character.Force)))
                {
                    var sbsub = new StringBuilder();
                    foreach (var s in chara.Force.Soldiers)
                    {
                        if (s.IsEmptySlot)
                        {
                            sbsub.Append($"|{EmptySlotMark}");
                        }
                        else
                        {
                            sbsub.Append($"|{s.Level},");
                            sbsub.Append($"{(s.Experience == 0? "" : s.Experience)},");
                            sbsub.Append($"{(s.HpFloat == s.MaxHp ? "" : s.HpFloat)}");
                        }
                    }
                    sb.Append(sbsub.ToString()).Append(delimiter);
                }
                else
                {
                   sb.Append(JsonConvert.SerializeObject(prop.Value)).Append(delimiter);
                }
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public static SavedCharacter Deserialize(string[] header, string line)
    {
        var values = line.Split('\t');
        var chara = new SavedCharacter
        {
            CountryId = int.Parse(values[0]),
            MemberOrderIndex = int.Parse(values[1]),
        };
        var character = new Character();
        var characterType = character.GetType();
        for (int j = 2; j < header.Length; j++)
        {
            var propName = header[j];
            var prop = characterType.GetProperty(propName);
            if (prop == null)
            {
                Debug.LogWarning($"Property not found: {propName}");
                continue;
            }

            if (propName.Equals(nameof(global::Character.Force)))
            {
                var field = values[j];
                // 新しい形式の場合
                if (!field.StartsWith("{"))
                {
                    var soldiers = field.Split('|', StringSplitOptions.RemoveEmptyEntries);
                    var force = new Force
                    {
                        Soldiers = new Soldier[soldiers.Length],
                    };
                    for (int k = 0; k < soldiers.Length; k++)
                    {
                        var soldier = soldiers[k];
                        if (soldier == EmptySlotMark)
                        {
                            force.Soldiers[k] = new Soldier { IsEmptySlot = true };
                        }
                        else
                        {
                            var values2 = soldier.Split(',');
                            var s = new Soldier();
                            s.Level = int.Parse(values2[0]);
                            s.Experience = values2[1] != "" ? float.Parse(values2[1]) : 0;
                            s.HpFloat = values2[2] != "" ? float.Parse(values2[2]) : s.MaxHp;
                            force.Soldiers[k] = s;
                        }
                    }
                    character.Force = force;
                    continue;
                }
            }

            // has setter
            if (prop.CanWrite)
            {
                var type = prop.PropertyType;
                var value = JsonConvert.DeserializeObject(values[j], type);
                prop.SetValue(character, value);
            }
        }
        chara.Character = character;
        return chara;
    }
}
