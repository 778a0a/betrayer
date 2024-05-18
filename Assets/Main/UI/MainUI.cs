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
        HideAllUI();
        StrategyPhase.Root.style.display = DisplayStyle.Flex;
        StrategyPhase.Refresh();
        currentScreen = StrategyPhase;
    }

    public void ShowIndividualUI()
    {
        HideAllUI();
        IndividualPhase.Root.style.display = DisplayStyle.Flex;
        currentScreen = IndividualPhase;
    }

    public void ShowMartialUI()
    {
        HideAllUI();
        MartialPhase.Root.style.display = DisplayStyle.Flex;
        currentScreen = MartialPhase;
    }


    private IScreen prevScreenForCountryScreen;
    public void ShowCountryInfoScreen()
    {
        if (currentScreen == CountryInfo) return;

        prevScreenForCountryScreen = currentScreen;
        HideAllUI();
        CountryInfo.Root.style.display = DisplayStyle.Flex;
        currentScreen = CountryInfo;

        CountryInfo.CloseButtonClicked += OnCloseButtonClicked;
        void OnCloseButtonClicked(object sender, EventArgs e)
        {
            CountryInfo.CloseButtonClicked -= OnCloseButtonClicked;
            CountryInfo.Root.style.display = DisplayStyle.None;
            prevScreenForCountryScreen.Root.style.display = DisplayStyle.Flex;
            currentScreen = prevScreenForCountryScreen;
        }
    }

    public Awaitable<Character> ShowSearchResult(Character[] charas, WorldData world)
    {
        HideAllUI();
        SelectCharacter.Root.style.display = DisplayStyle.Flex;
        
        return SelectCharacter.Show(
            "採用する人物をクリックしてください。",
            "採用しない",
            charas,
            world);
    }

    public Awaitable<Character> ShowFireVassalUI(Country country, WorldData world)
    {
        HideAllUI();
        SelectCharacter.Root.style.display = DisplayStyle.Flex;

        return SelectCharacter.Show(
            "追放する人物をクリックしてください。",
            "キャンセル",
            country.Vassals,
            world);
    }

    /// <summary>
    /// 侵攻画面 攻撃側選択UIを表示します。
    /// </summary>
    public Awaitable<Character> ShowSelectAttackerScreen(Country country, WorldData world)
    {
        HideAllUI();
        SelectCharacter.Root.style.display = DisplayStyle.Flex;

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
    public Awaitable<Character> ShowSubdueScreen(Country country, WorldData world)
    {
        HideAllUI();
        SelectCharacter.Root.style.display = DisplayStyle.Flex;

        return SelectCharacter.Show(
            "討伐する人物をクリックしてください。",
            "キャンセル",
            country.Vassals,
            world);
    }

    public Awaitable<Character> ShowSelectDefenderScreen(
        Area attackerArea,
        Country attackerCountry,
        Character attacker,
        Area defenderArea,
        Country defenderCountry,
        WorldData world)
    {
        HideAllUI();
        SelectCharacter.Root.style.display = DisplayStyle.Flex;
        // TODO
        return SelectCharacter.Show(
            "防衛する人物をクリックしてください。",
            "放棄する",
            defenderCountry.Members.ToList(),
            world,
            c => !c.IsAttacked);
    }


    public Awaitable<bool> ShowOrganizeScreen(Country country, WorldData world)
    {
        HideAllUI();
        Organize.Root.style.display = DisplayStyle.Flex;

        return Organize.Show(
            country,
            world);
    }

    public Awaitable<Character> ShowSelectProvokingTargetScreen(
        Country[] candidateCountries,
        WorldData world)
    {
        HideAllUI();
        SelectCharacter.Root.style.display = DisplayStyle.Flex;

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

    public Awaitable<Country> ShowSelectAllyScreen(Country country, WorldData world)
    {
        HideAllUI();
        SelectCountry.Root.style.display = DisplayStyle.Flex;
        
        return SelectCountry.Show(
            "同盟を結ぶ国を選択してください。",
            world,
            country =>
            {
                if (country.Ally != null)
                {
                    return (false, "すでに別の国と同盟を結んでいます。");
                }
                else
                {
                    return (true, "この国と同盟を結びますか？");
                }
            });
    }

    public Awaitable<Country> ShowGetJobScreen(WorldData world)
    {
        HideAllUI();
        SelectCountry.Root.style.display = DisplayStyle.Flex;

        return SelectCountry.Show(
            "仕官する国を選択してください。",
            world,
            country =>
            {
                if (country.Vassals.Count >= country.VassalCountMax)
                {
                    return (false, "この国はこれ以上配下を雇えません。");
                }
                else
                {
                    return (true, "この国に仕官しますか？");
                }
            });
    }

    public Awaitable<Area> ShowSelectAreaScreen(List<Area> targetAreas, WorldData world)
    {
        HideAllUI();
        SelectArea.Root.style.display = DisplayStyle.Flex;

        return SelectArea.Show(
            "侵攻する地域を選択してください。",
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

    public Awaitable<bool> ShowRespondAllyRequestScreen(
        Country reqester,
        WorldData world)
    {
        HideAllUI();
        RespondCountryAction.Root.style.display = DisplayStyle.Flex;

        return RespondCountryAction.Show(
            "以下の勢力から同盟の申し込みがありました。",
            "受諾",
            "拒否",
            reqester,
            world);
    }

    public Awaitable<bool> ShowRespondJobOfferScreen(
        Country reqester,
        WorldData world)
    {
        HideAllUI();
        RespondCountryAction.Root.style.display = DisplayStyle.Flex;

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
}

public class FaceImageManager
{
    public static FaceImageManager Instance { get; } = new();

    private Dictionary<string, Texture2D> cacheImages = new();
    public void ClearCache() => cacheImages.Clear();

    public Texture2D GetImage(Character chara)
    {
        var path = chara.debugImagePath;
        if (path == null) return null;

        if (cacheImages.TryGetValue(path, out var tex))
        {
            return tex;
        }

        var bytes = System.IO.File.ReadAllBytes(path);
        tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);
        cacheImages[path] = tex;
        return tex;
    }

}
