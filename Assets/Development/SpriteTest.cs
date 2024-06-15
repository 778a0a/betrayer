using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class SpriteTest : MonoBehaviour
{
    [SerializeField] private Texture2D texture;
    Color[] colors = new Color[]
    {
        Util.Color("#fff"),
        Util.Color("#000"),
        Util.Color("#80f"),
        Util.Color("#f0f"),
        Util.Color("#f00"),
        Util.Color("#f80"),
        Util.Color("#ff0"),
        Util.Color("#8f0"),
        Util.Color("#0a0"),
        Util.Color("#0ff"),
        Util.Color("#04f"),
        Util.Color("#666"),
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var r = GetComponent<SpriteRenderer>();
        //var texture = r.sprite.texture;
        var originalPixels = texture.GetPixels32();
        Debug.Log(originalPixels.Length);
        Debug.Log(texture.format);

        var replaceColors = new[]
        {
            (Util.Color("#8f563b"), 0.2f, 0.7f), // 服
            (Util.Color("#663931"), 0.8f, 0.8f), // 靴、スカーフ、帽子
            (Util.Color("#696a6a"), 0.8f, 0.8f), // 胴の鎧
            (Util.Color("#595652"), 0.3f, 0.7f), // その他の鎧
        };
        Color Merge(Color original, Color newColor)
        {
            foreach (var (targetColor, newColorWeight, newColorWeightHighLevel) in replaceColors)
            {
                if (original != targetColor) continue;
                var w = newColorWeight;
                if (newColor == colors[0] || newColor == colors[1])
                {
                    w = newColorWeightHighLevel;
                }

                var oldColorWeight = 1 - w;
                var r = original.r * oldColorWeight + newColor.r * w;
                var g = original.g * oldColorWeight + newColor.g * w;
                var b = original.b * oldColorWeight + newColor.b * w;
                return new Color(r, g, b);
            }
            return original;
        }
        for (int i = 0; i < colors.Length; i++)
        {
            var textureCopy = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount, false);
            Graphics.CopyTexture(texture, textureCopy);
            textureCopy.filterMode = FilterMode.Point;

            var replacedPixels = textureCopy.GetPixels()
                .Select(p => Merge(p, colors[i]))
                .ToArray();
            textureCopy.SetPixels(replacedPixels);
            textureCopy.Apply();
            
            // 新しいテクスチャからスプライトを作成
            Rect rect = new(0, 0, textureCopy.width, textureCopy.height);
            Vector2 pivot = new(0.5f, 0.5f);
            Sprite newSprite = Sprite.Create(textureCopy, rect, pivot);

            var newGameObject = new GameObject("NewSprite");
            var spriteRenderer = newGameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = newSprite;
            newGameObject.transform.position = new Vector3(i*0.3f, 0, 0);
        }
    }
}
