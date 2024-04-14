using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AreaInfoEditorWindow : EditorWindow
{
    private WorldData world;
    private Grid grid;

    private Area targetArea;
    private Country targetCountry;

    private bool isLocked = false;

    [MenuItem("Window/タイル情報")]
    public static void ShowWindow()
    {
        GetWindow<AreaInfoEditorWindow>("タイル情報");
    }

    void OnEnable()
    {
        var helper = FindFirstObjectByType<TilemapHelper>();
        world = SaveData.LoadWorldData(helper);
        grid = FindFirstObjectByType<Grid>();

        SceneView.duringSceneGui += DuringSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    void DuringSceneGUI(SceneView sceneView)
    {

        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        if (hit.collider != null)
        {
            var posGrid = grid.WorldToCell(hit.point);
            var pos = MapPosition.FromGrid(posGrid);
            if (!isLocked)
            {
                targetArea = world.Map.GetArea(pos);
                targetCountry = world.CountryOf(targetArea);
            }
            Repaint();
        }
    }

    void OnGUI()
    {
        // lキーが押されたらロック状態をトグルする。
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.L)
        {
            isLocked = !isLocked;
        }
        GUILayout.Label("ロック: " + isLocked);
        if (GUILayout.Button("保存"))
        {
            SaveData.SaveWorldData(world);
        }

        if (targetArea == null)
        {
            EditorGUILayout.LabelField("タイルを選択してください");
            return;
        }
        Debug.Assert(targetCountry != null);

        GUILayout.Label($"エリア {targetArea}");
        GUILayout.Label($"所有国 {targetCountry}");
        // 君主
        GUILayout.Label("君主");
        DrawCharacter(targetCountry.Ruler);
        // 配下
        GUILayout.Label("配下");
        foreach (var vassal in targetCountry.Vassals)
        {
            DrawCharacter(vassal);
        }
    }

    private void DrawCharacter(Character chara)
    {
        GUILayout.Label(chara.ToString());
        // Attack Defense Intelligenceをテキストボックスで並べる。
        // それぞれの値を変更できるようにする。
        chara.Attack = EditorGUILayout.IntField("Attack", chara.Attack);
        chara.Defense = EditorGUILayout.IntField("Defense", chara.Defense);
        chara.Intelligence = EditorGUILayout.IntField("Intelligence", chara.Intelligence);
        GUILayout.Space(5);
    }

}
