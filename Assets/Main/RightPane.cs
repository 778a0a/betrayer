using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public partial class RightPane : MonoBehaviour
{
    public event EventHandler<RightPaneButton> RightPaneButtonClick;
    public enum RightPaneButton
    {
        ToggleDebugUI,
        NextPhase,
        NextTurn,
        Auto,
        Hold,
    }

    [SerializeField] private VisualTreeAsset[] screenVisualTreeAssets;

    public IndividualPhaseUI IndividualPhaseUI { get; private set; }
    public StrategyPhaseUI StrategyPhaseUI { get; private set; }
    public MartialPhaseUI MartialPhaseUI { get; private set; }
    public SelectCharacterUI SelectCharacterUI { get; private set; }
    public CountryInfo CountryInfo { get; private set; }

    private IScreen[] _Screens;
    private IScreen[] Screens => _Screens ??= GetType().GetProperties()
        .Where(p => p.PropertyType.GetInterface(nameof(IScreen)) != null)
        .Select(p => (IScreen)p.GetValue(this))
        .ToArray();

    private void OnEnable()
    {
        InitializeDocument();

        var screenProps = GetType()
            .GetProperties()
            .Where(p => p.PropertyType.GetInterface(nameof(IScreen)) != null);
        var assetNotFound = false;
        Debug.Log(screenVisualTreeAssets.Length);
        foreach (var screenProp in screenProps)
        {
            var asset = screenVisualTreeAssets.FirstOrDefault(a => a.name == screenProp.Name);
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

        buttonToggleDebugUI.clicked += () => RightPaneButtonClick?.Invoke(this, RightPaneButton.ToggleDebugUI);
        buttonNextPhase.clicked += () => RightPaneButtonClick?.Invoke(this, RightPaneButton.NextPhase);
        buttonNextTurn.clicked += () => RightPaneButtonClick?.Invoke(this, RightPaneButton.NextTurn);
        buttonAuto.clicked += () => RightPaneButtonClick?.Invoke(this, RightPaneButton.Auto);
        buttonHold.clicked += () => RightPaneButtonClick?.Invoke(this, RightPaneButton.Hold);
    }

    public void ShowStrategyUI()
    {
        HideAllUI();
        StrategyPhaseUI.Root.style.display = DisplayStyle.Flex;
        StrategyPhaseUI.Refresh();
    }

    public void ShowIndividualUI()
    {
        HideAllUI();
        IndividualPhaseUI.Root.style.display = DisplayStyle.Flex;
    }

    public void ShowMartialUI()
    {
        HideAllUI();
        MartialPhaseUI.Root.style.display = DisplayStyle.Flex;
    }

    public void ShowCountryInfo()
    {
        HideAllUI();
        CountryInfo.Root.style.display = DisplayStyle.Flex;
    }

    public Awaitable<Character> ShowSearchResult(Character[] charas, WorldData world)
    {
        HideAllUI();
        SelectCharacterUI.Root.style.display = DisplayStyle.Flex;
        
        return SelectCharacterUI.Show(
            "採用する人物をクリックしてください。",
            "採用しない",
            charas,
            world);
    }

    public Awaitable<Character> ShowFireVassalUI(Country country, WorldData world)
    {
        HideAllUI();
        SelectCharacterUI.Root.style.display = DisplayStyle.Flex;

        return SelectCharacterUI.Show(
            "追放する人物をクリックしてください。",
            "キャンセル",
            country.Vassals,
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