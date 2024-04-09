using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap tilemap;

    [SerializeField] private TilesHolder tilesHolder;

    [SerializeField] private TilemapData initialTilemapData;

    [SerializeField] private WorldData world;

    [SerializeField] private float wait = 1;

    [SerializeField] private TilemapHelper tilemapHelper;

    private PhaseManager phases;

    // Start is called before the first frame update
    void Start()
    {
        world = DefaultData.InitializeDefaultData(initialTilemapData, tilemapHelper);
        phases = new PhaseManager(world);
        DrawCountryTile();

        SaveCharacterData(world);

        StartCoroutine(DoMainLoop());
    }

    // Update is called once per frame
    private IEnumerator DoMainLoop()
    {
        while (true)
        {
            phases.Start.Phase();
            phases.Income.Phase();
            phases.PersonalAction.Phase();
            phases.StrategyAction.Phase();
            DrawCountryTile();
            if (world.Countries.Count == 1)
            {
                Debug.Log($"ゲーム終了 勝者: {world.Countries[0]}");
                yield break;
            }
            yield return new WaitForSeconds(wait);
        }
    }

    private void DrawCountryTile()
    {
        var index2Tile = tilesHolder.countries;
        foreach (var country in world.Countries)
        {
            var colorIndex = country.ColorIndex;
            foreach (var area in country.Areas)
            {
                var pos = area.Position;
                tilemap.SetTile(pos.Vector3Int, index2Tile[country.ColorIndex]);
            }
        }
    }

    private void SaveCharacterData(WorldData world)
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




        // json形式で保存する。
        var csv = SavedCharacter.CreateCsv(charas);
        var path = Application.dataPath + "/Development/SavedData/character_data.csv";
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, csv);
    }

}

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
}
