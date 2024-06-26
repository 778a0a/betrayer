using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public partial class MainUI : MonoBehaviour
{
    public event EventHandler<MainUIButton> MainUIButtonClick;
    public enum MainUIButton
    {
        ToggleDebugUI,
        NextPhase,
        NextTurn,
        Auto,
        Hold,
    }

    [SerializeField] private VisualTreeAsset[] screenVisualTreeAssets;

    public CountryInfoScreen CountryInfo { get; private set; }
    public IndividualPhaseScreen IndividualPhase { get; private set; }
    public StrategyPhaseScreen StrategyPhase { get; private set; }
    public MartialPhaseScreen MartialPhase { get; private set; }
    public SelectCharacterScreen SelectCharacter { get; private set; }
    public SelectCountryScreen SelectCountry { get; private set; }
    public SelectAreaScreen SelectArea { get; private set; }
    public RespondCountryActionScreen RespondCountryAction { get; private set; }
    public OrganizeScreen Organize { get; private set; }

    private IScreen currentScreen;

    private void InitializeScreens()
    {
        var screenProps = GetType()
            .GetProperties()
            .Where(p => p.PropertyType.GetInterface(nameof(IScreen)) != null);
        var assetNotFound = false;
        foreach (var screenProp in screenProps)
        {
            var asset = screenVisualTreeAssets.FirstOrDefault(a => a.name == $"{screenProp.Name}Screen");
            if (asset == null)
            {
                Debug.LogError($"VisualTreeAsset not found: {screenProp.Name}");
                assetNotFound = true;
                continue;
            }

            var element = asset.Instantiate();
            var constructor = screenProp.PropertyType.GetConstructor(new[] { typeof(VisualElement) });
            var screen = constructor.Invoke(new[] { element }) as IScreen;
            screen.Initialize();
            screenProp.SetValue(this, screen);
            UIContainer.Add(element);
        }
        if (assetNotFound || screenProps.Any(p => p.GetValue(this) == null))
        {
            // ゲームを強制終了する。
            Debug.LogError("UI Documentが足りないので強制終了します。");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    private void OnEnable()
    {
        InitializeDocument();
        InitializeScreens();
        BattleDialog.Initialize();
        BattleDialog.Root.style.display = DisplayStyle.None;

        buttonToggleDebugUI.clicked += () => MainUIButtonClick?.Invoke(this, MainUIButton.ToggleDebugUI);
        buttonNextPhase.clicked += () => MainUIButtonClick?.Invoke(this, MainUIButton.NextPhase);
        buttonNextTurn.clicked += () => MainUIButtonClick?.Invoke(this, MainUIButton.NextTurn);
        buttonAuto.clicked += () => MainUIButtonClick?.Invoke(this, MainUIButton.Auto);
        buttonHold.clicked += () => MainUIButtonClick?.Invoke(this, MainUIButton.Hold);
    }

    public void ShowStrategyUI()
    {
        ShowScreen(StrategyPhase);
        StrategyPhase.Refresh();
    }

    public void ShowIndividualUI()
    {
        ShowScreen(IndividualPhase);
    }

    public void ShowMartialUI()
    {
        ShowScreen(MartialPhase);
    }


    private IScreen prevScreenForCountryScreen;
    public void ShowCountryInfoScreen()
    {
        if (currentScreen == CountryInfo) return;

        prevScreenForCountryScreen = currentScreen;
        ShowScreen(CountryInfo);

        CountryInfo.CloseButtonClicked += OnCloseButtonClicked;
        void OnCloseButtonClicked(object sender, EventArgs e)
        {
            CountryInfo.CloseButtonClicked -= OnCloseButtonClicked;
            CountryInfo.Root.style.display = DisplayStyle.None;
            prevScreenForCountryScreen.Root.style.display = DisplayStyle.Flex;
            currentScreen = prevScreenForCountryScreen;
        }
    }

    public ValueTask<Character> ShowSearchResult(Character[] charas, WorldData world)
    {
        ShowScreen(SelectCharacter);
        
        return SelectCharacter.Show(
            "採用する人物をクリックしてください。",
            "採用しない",
            charas,
            world,
            _ => true);
    }

    public ValueTask<Character> ShowFireVassalUI(Country country, WorldData world)
    {
        ShowScreen(SelectCharacter);

        return SelectCharacter.Show(
            "追放する人物をクリックしてください。",
            "キャンセル",
            country.Vassals,
            world,
            _ => true);
    }

    /// <summary>
    /// 侵攻画面 攻撃側選択UIを表示します。
    /// </summary>
    public ValueTask<Character> ShowSelectAttackerScreen(Country country, WorldData world)
    {
        ShowScreen(SelectCharacter);

        return SelectCharacter.Show(
            "侵攻を行う人物をクリックしてください。",
            "キャンセル",
            country.Members.ToList(),
            world,
            c => !c.IsAttacked);
    }

    /// <summary>
    /// 討伐画面を表示します。
    /// </summary>
    public ValueTask<Character> ShowSubdueScreen(Country country, WorldData world)
    {
        ShowScreen(SelectCharacter);

        return SelectCharacter.Show(
            "討伐する人物をクリックしてください。",
            "キャンセル",
            country.Vassals,
            world,
            _ => true);
    }

    /// <summary>
    /// 私闘画面を表示します。
    /// </summary>
    public ValueTask<Character> ShowPrivateFightScreen(IList<Character> targets, WorldData world)
    {
        ShowScreen(SelectCharacter);

        return SelectCharacter.Show(
            "攻撃する人物をクリックしてください。",
            "キャンセル",
            targets,
            world,
            _ => true);
    }

    public ValueTask<Character> ShowSelectDefenderScreen(
        Area attackerArea,
        Country attackerCountry,
        Character attacker,
        Area defenderArea,
        Country defenderCountry,
        WorldData world)
    {
        ShowScreen(SelectCharacter);
        
        // TODO
        return SelectCharacter.Show(
            "防衛する人物をクリックしてください。",
            "放棄する",
            defenderCountry.Members.ToList(),
            world,
            c => !c.IsAttacked);
    }


    public ValueTask<bool> ShowOrganizeScreen(Country country, WorldData world)
    {
        ShowScreen(Organize);

        return Organize.Show(
            country,
            world);
    }

    public ValueTask<Character> ShowSelectProvokingTargetScreen(
        Country[] candidateCountries,
        WorldData world)
    {
        ShowScreen(SelectCharacter);

        return SelectCharacter.ShowForProvoke(
            "挑発する国を選択してください。",
            world,
            area =>
            {
                if (candidateCountries.Any(c => c.Areas.Contains(area)))
                {
                    return (true, "挑発する人物を選択してください。");
                }
                else
                {
                    return (false, "この国には挑発できません。");
                }
            });
    }

    public ValueTask<Country> ShowSelectAllyScreen(Country country, WorldData world)
    {
        ShowScreen(SelectCountry);
        
        return SelectCountry.Show(
            "同盟を結ぶ国を選択してください。",
            world,
            target =>
            {
                if (target == country) return (false, "自国です。");
                if (target.Ally != null) return (false, "すでに別の国と同盟を結んでいます。");
                else return (true, "この国と同盟を結びますか？");
            });
    }

    public ValueTask<Country> ShowGetJobScreen(WorldData world)
    {
        ShowScreen(SelectCountry);

        return SelectCountry.Show(
            "仕官する国を選択してください。",
            world,
            country =>
            {
                //if (country.Vassals.Count >= country.VassalCountMax)
                if (country.Vassals.Count >= Country.VassalCountMaxLimit)
                {
                    return (false, "この国はこれ以上配下を雇えません。");
                }
                else
                {
                    return (true, "この国に仕官しますか？");
                }
            });
    }

    public ValueTask<Area> ShowSelectAreaScreen(
        IList<Area> targetAreas,
        WorldData world,
        string text = "侵攻する地域を選択してください。")
    {
        ShowScreen(SelectArea);

        return SelectArea.Show(
            text,
            world,
            a =>
            {
                if (targetAreas.Contains(a))
                {
                    return (true, "この地域に侵攻しますか？");
                }
                else
                {
                    return (false, "この地域には侵攻できません。");
                }

            });
    }

    public ValueTask<bool> ShowRespondAllyRequestScreen(
        Country reqester,
        WorldData world)
    {
        ShowScreen(RespondCountryAction);

        return RespondCountryAction.Show(
            "以下の勢力から同盟の申し込みがありました。",
            "受諾",
            "拒否",
            reqester,
            world);
    }

    public ValueTask<bool> ShowRespondJobOfferScreen(
        Country reqester,
        WorldData world)
    {
        ShowScreen(RespondCountryAction);

        return RespondCountryAction.Show(
            "以下の勢力から仕官の誘いがありました。",
            "受諾",
            "拒否",
            reqester,
            world);
    }

    public void HideAllUI()
    {
        foreach (var item in UIContainer.Children())
        {
            item.style.display = DisplayStyle.None;
        }
    }

    private void ShowScreen(IScreen screen)
    {
        HideAllUI();
        screen.Root.style.display = DisplayStyle.Flex;
        currentScreen = screen;
    }
}

public class FaceImageManager
{
    public static FaceImageManager Instance { get; } = new();

    private Dictionary<string, Texture2D> cacheImages = new();
    public void ClearCache() => cacheImages.Clear();

    public Texture2D GetImage(Character chara) => GetImage(chara.Id);
    public Texture2D GetImage(int charaId)
    {
        var path = $"CharacterImages/{charaId:0000}";
        if (cacheImages.TryGetValue(path, out var tex))
        {
            return tex;
        }

        tex = Resources.Load<Texture2D>(path);
        cacheImages[path] = tex;
        return tex;
    }

}

public class SoldierImageManager
{
    public static SoldierImageManager Instance { get; } = new();

    private static readonly Color[] colors = new Color[]
    {
        Util.Color("#666"),
        Util.Color("#04f"),
        Util.Color("#0ff"),
        Util.Color("#0a0"),
        Util.Color("#8f0"),
        Util.Color("#ff0"),
        Util.Color("#f80"),
        Util.Color("#f00"),
        Util.Color("#f0f"),
        Util.Color("#80f"),
        Util.Color("#000"),
        Util.Color("#fff"),
    };

    private readonly Dictionary<int, Texture2D> textures = new();
    private readonly Dictionary<int, Sprite> sprites = new();
    private Texture2D emptyTexture;

    public void Initialize(Texture2D original)
    {
        emptyTexture = new Texture2D(0, 0);
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
            var tex = new Texture2D(
                original.width,
                original.height,
                original.format,
                original.mipmapCount,
                false);
            Graphics.CopyTexture(original, tex);
            tex.filterMode = FilterMode.Point;
            var replacedPixels = tex.GetPixels()
                .Select(p => Merge(p, colors[i]))
                .ToArray();
            tex.SetPixels(replacedPixels);
            tex.Apply();
            textures[i] = tex;

            var rect = new Rect(0, 0, tex.width, tex.height);
            var pivot = new Vector2(0.5f, 0.5f);
            var newSprite = Sprite.Create(tex, rect, pivot);
            sprites[i] = newSprite;
        }
    }

    public Color GetColor(int level)
    {
        if (level >= colors.Length) level = colors.Length - 1;
        return colors[level];
    }

    public Texture2D GetTexture(int level)
    {
        if (level >= colors.Length) level = colors.Length - 1;
        return textures[level];
    }

    public Sprite GetSprite(int level)
    {
        if (level >= colors.Length) level = colors.Length - 1;
        return sprites[level];
    }

    public Texture2D GetEmptyTexture()
    {
        return emptyTexture;
    }
}
