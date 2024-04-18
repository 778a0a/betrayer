using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    private bool showFree = false;
    private Vector2 scrollPosition = Vector2.zero;
    void OnGUI()
    {
        // lキーが押されたらロック状態をトグルする。
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.L)
        {
            isLocked = !isLocked;
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("ロック: " + isLocked);
        if (GUILayout.Button("ロックトグル", GUILayout.Width(100)))
        {
            isLocked = !isLocked;
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("所属なしを表示"))
        {
            showFree = !showFree;
        }


        if (GUILayout.Button("保存"))
        {
            SaveData.SaveWorldData(world);
        }

        if (showFree)
        {
            DrawFree();
            return;
        }


        if (targetArea == null)
        {
            EditorGUILayout.LabelField("タイルを選択してください");
            return;
        }
        Debug.Assert(targetCountry != null);

        GUILayout.Label($"エリア {targetArea}");
        GUILayout.Label($"所有国 {targetCountry}");
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        // 君主
        GUILayout.Label("君主");
        DrawCharacter(targetCountry.Ruler);
        // 配下
        GUILayout.Label("配下");
        foreach (var vassal in targetCountry.Vassals)
        {
            DrawCharacter(vassal);
        }
        GUILayout.EndScrollView();
    }

    private int currentPage = 0;
    private int characterPerPage = 4;
    private void DrawFree()
    {
        GUILayout.Label("所属なし");
        var frees = world.Characters
            .Where(c => world.CountryOf(c) == null)
            .OrderBy(c => c.debugMemo)
            .ToArray();
        var pageCount = frees.Length / characterPerPage;
        
        GUILayout.BeginHorizontal();
        // 前後ボタン
        if (GUILayout.Button("前へ"))
        {
            currentPage = Mathf.Max(0, currentPage - 1);
        }
        if (GUILayout.Button("次へ"))
        {
            currentPage = Mathf.Min(pageCount, currentPage + 1);
        }
        // 現在のページとページ数
        GUILayout.Label($"{currentPage + 1}/{pageCount + 1}");
        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        var startIndex = currentPage * characterPerPage;
        var endIndex = Mathf.Min((currentPage + 1) * characterPerPage, frees.Length);
        for (int i = startIndex; i < endIndex; i++)
        {
            DrawCharacter(frees[i]);
        }

        GUILayout.EndScrollView();

    }

    private void DrawCharacter(Character chara)
    {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label($"ID: {chara.Id}");
        GUILayout.Label($"{chara}");
        // Attack Defense Intelligenceをテキストボックスで並べる。
        // それぞれの値を変更できるようにする。
        chara.Attack = EditorGUILayout.IntField("Attack", chara.Attack);
        chara.Defense = EditorGUILayout.IntField("Defense", chara.Defense);
        chara.Intelligence = EditorGUILayout.IntField("Intelligence", chara.Intelligence);
        chara.debugMemo = EditorGUILayout.TextField("メモ", chara.debugMemo);
        chara.debugImagePath = EditorGUILayout.TextField("顔画像", chara.debugImagePath);
        chara.debugImagePath = DrawDropArea(chara.debugImagePath);
        
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        if (chara.debugImagePath != null)
        {
            var image = File.ReadAllBytes(chara.debugImagePath);
            var tex = new Texture2D(1, 1);
            tex.LoadImage(image);
            GUILayout.Box(tex, new GUIStyle() { fixedWidth = 200, fixedHeight = 200 });
        }
        else
        {
            GUILayout.Box("No Image", new GUIStyle() { fixedWidth = 200, fixedHeight = 200 });
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
    }

    private string DrawDropArea(string path)
    {
        var dropArea = GUILayoutUtility.GetRect(100f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drop Image Here");
        var e = Event.current;
        switch (e.type)
        {
            // なぜかExplorerからドロップしてもDragPerformが呼ばれないので、
            // DragUpdatedで更新する。
            case EventType.DragUpdated:
                //case EventType.DragPerform:
                if (!dropArea.Contains(e.mousePosition)) break;
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                DragAndDrop.AcceptDrag();
                DragAndDrop.activeControlID = 0;
                Event.current.Use();
                foreach (var p in DragAndDrop.paths)
                {
                    Debug.Log($"Accepted: {p}");
                    return p;
                }
                break;
        }
        return path;
    }

}
