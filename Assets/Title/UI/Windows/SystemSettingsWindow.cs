using System;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SystemSettingsWindow : IWindow
{
    public SystemSettingsManager SystemSettings => SystemSettingsManager.Instance;
    public LocalizationManager L { get; set; }

    public void Initialize()
    {
        L.Register(this);

        var orientations = Util.EnumArray<OrientationSetting>();
        comboOrientation.index = Array.IndexOf(orientations, SystemSettings.Orientation);
        comboOrientation.RegisterValueChangedCallback(e =>
        {
            SystemSettings.Orientation = orientations[comboOrientation.index];
            SystemSettings.ApplyOrientation();
        });

        var layouts = Util.EnumArray<LayoutSetting>();
        comboLayout.index = Array.IndexOf(layouts, SystemSettings.Layout);
        comboLayout.RegisterValueChangedCallback(e =>
        {
            SystemSettings.Layout = layouts[comboLayout.index];
        });

        CloseButton.clicked += () => Root.style.display = DisplayStyle.None;
    }

    public void Show()
    {
        Root.style.display = DisplayStyle.Flex;
    }
}

public class SystemSettingsManager
{
    public static SystemSettingsManager Instance { get; } = new SystemSettingsManager();

    public OrientationSetting Orientation
    {
        get => (OrientationSetting)PlayerPrefs.GetInt(nameof(Orientation), (int)OrientationSetting.UserSetting);
        set => PlayerPrefs.SetInt(nameof(Orientation), (int)value);
    }

    public LayoutSetting Layout
    {
        get => (LayoutSetting)PlayerPrefs.GetInt(nameof(Layout), (int)LayoutSetting.Auto);
        set => PlayerPrefs.SetInt(nameof(Layout), (int)value);
    }

    public void ApplyOrientation()
    {
        var orientation = Orientation;
        switch (orientation)
        {
            case OrientationSetting.UserSetting:
                Screen.orientation = ScreenOrientation.AutoRotation;
                break;
            case OrientationSetting.Sensor:
                Screen.orientation = ScreenOrientation.AutoRotation;
                //Screen.autorotateToPortrait = true;
                //Screen.autorotateToPortraitUpsideDown = true;
                //Screen.autorotateToLandscapeLeft = true;
                //Screen.autorotateToLandscapeRight = true;
                break;
            case OrientationSetting.Portrait:
                Screen.orientation = ScreenOrientation.Portrait;
                break;
            case OrientationSetting.PortraitUpsideDown:
                Screen.orientation = ScreenOrientation.PortraitUpsideDown;
                break;
            case OrientationSetting.LandscapeLeft:
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                break;
            case OrientationSetting.LandscapeRight:
                Screen.orientation = ScreenOrientation.LandscapeRight;
                break;
        }
    }
}

public enum OrientationSetting
{
    UserSetting = 0,
    Sensor,
    Portrait,
    PortraitUpsideDown,
    LandscapeLeft,
    LandscapeRight,
}

public enum LayoutSetting
{
    Auto = 0,
    Columns,
    Rows,
}