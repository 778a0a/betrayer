using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class SavedCharacter
{
    public int CountryId { get; set; }
    public int MemberOrderIndex { get; set; }
    public Character Character { get; set; }

    public bool IsRuler => !IsFree && MemberOrderIndex == 0;
    public bool IsFree => CountryId == -1;

    public static string CreateCsv(List<SavedCharacter> charas)
    {
        var json = JsonConvert.SerializeObject(charas);
        var list = JsonConvert.DeserializeObject<List<JObject>>(json);
        var sb = new System.Text.StringBuilder();

        var delimiter = "\t";
        // ヘッダー
        sb.Append(nameof(CountryId)).Append(delimiter);
        sb.Append(nameof(MemberOrderIndex)).Append(delimiter);
        foreach (JProperty prop in list[0][nameof(Character)])
        {
            sb.Append(prop.Name).Append(delimiter);
        }
        sb.AppendLine();

        // 中身
        foreach (var obj in list)
        {
            sb.Append(obj[nameof(CountryId)]).Append(delimiter);
            sb.Append(obj[nameof(MemberOrderIndex)]).Append(delimiter);
            foreach (JProperty prop in obj[nameof(Character)])
            {
                sb.Append(JsonConvert.SerializeObject(prop.Value)).Append(delimiter);
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public static List<SavedCharacter> LoadCharacterData(string csv)
    {
        //var csv = File.ReadAllText(path);
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
            var values = line.Split('\t');
            var chara = new SavedCharacter
            {
                CountryId = int.Parse(values[0]),
                MemberOrderIndex = int.Parse(values[1]),
            };
            var character = new Character();
            for (int j = 2; j < header.Length; j++)
            {
                var propName = header[j];
                var prop = character.GetType().GetProperty(propName);
                // has setter
                if (prop.CanWrite)
                {
                    var type = prop.PropertyType;
                    var value = JsonConvert.DeserializeObject(values[j], type);
                    prop.SetValue(character, value);
                }
            }
            chara.Character = character;
            charas.Add(chara);
        }
        return charas;
    }
}
