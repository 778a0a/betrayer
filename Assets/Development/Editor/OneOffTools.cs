using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OneOffTools : EditorWindow
{
    [MenuItem("開発/ワンオフ/国タイル画像をタイルにセット")]
    public static void 国タイル画像をタイルにセット()
    {
        var tilePath = "Assets/Main/Tiles/CountryTile/Tile/";
        var imagePath = "Assets/Main/Tiles/CountryTile/Image/";

        var tileAssets = AssetDatabase.FindAssets("t:Tile", new[] { tilePath });

        foreach (var tileGUID in tileAssets)
        {
            var tileAssetPath = AssetDatabase.GUIDToAssetPath(tileGUID);
            var tile = AssetDatabase.LoadAssetAtPath<Tile>(tileAssetPath);
            Debug.Log($"processing {tile.name}");
            if (tile != null)
            {
                var spriteName = tile.name.Split('_')[0];
                var spritePath = imagePath + spriteName + ".png";
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                if (sprite != null)
                {
                    tile.sprite = sprite;
                    EditorUtility.SetDirty(tile);
                }
                else
                {
                    Debug.LogWarning($"Sprite not found for {tileAssetPath}");
                }
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
