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

    private void OnEnable()
    {
        InitializeDocument();
        IndividualPhaseUI.Initialize();
        StrategyPhaseUI.Initialize();
        MartialPhaseUI.Initialize();
        SelectCharacterUI.Initialize();
        CountryInfo.Initialize();

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