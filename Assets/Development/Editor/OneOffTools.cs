using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
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

    [MenuItem("開発/利用している顔グラ画像を連番jpg化する")]
    public static void 利用している顔グラ画像を連番jpg化する()
    {
        var outputDir = @"Assets\Resources\CharacterImages";

        var csv = Resources.Load<TextAsset>(SaveData.DefaultCsvPath).text;
        var charas = SavedCharacters.Deserialize(csv);

        try
        {
            AssetDatabase.StartAssetEditing();
            foreach (var chara in charas)
            {
                var id = chara.Character.Id.ToString("0000");
                var jpgPath = outputDir + $"/{id}.jpg";
                Debug.Log($"processing {id} {chara.Character.Name}");
                var pngPath = chara.Character.debugImagePath;
                var pngBytes = File.ReadAllBytes(pngPath);
                
                // リサイズして保存する。
                using var bmp = new Bitmap(new MemoryStream(pngBytes));
                var resized = new Bitmap(512, 512);
                using (var g = System.Drawing.Graphics.FromImage(resized))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(bmp, 0, 0, 512, 512);
                }
                resized.Save(jpgPath, ImageFormat.Jpeg);

                //var png = new Texture2D(2, 2);
                //png.LoadImage(pngBytes);
                //var jpg = png.EncodeToJPG(50);
                //File.WriteAllBytes(jpgPath, jpg);
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }
        Debug.Log("完了");
    }
}
