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
    public OrganizeScreen Organize { get; private set; }
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
        if (assetNotFound)
        {
            throw new Exception("VisualTreeAsset not found.");
        }
    }

    private void OnEnable()
    {
        InitializeDocument();
        InitializeScreens();

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
    }

    public void ShowIndividualUI()
    {
        HideAllUI();
        IndividualPhase.Root.style.display = DisplayStyle.Flex;
    }

    public void ShowMartialUI()
    {
        HideAllUI();
        MartialPhase.Root.style.display = DisplayStyle.Flex;
    }

    public void ShowCountryInfoScreen()
    {
        HideAllUI();
        CountryInfo.Root.style.display = DisplayStyle.Flex;
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

    public Awaitable<bool> ShowOrganizeScreen(Country country, WorldData world)
    {
        HideAllUI();
        Organize.Root.style.display = DisplayStyle.Flex;

        return Organize.Show(
            country,
            world);
    }

    private void HideAllUI()
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


public interface IScreen
{
    void Initialize();
}