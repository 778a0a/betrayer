using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;

public class LocalizationManager : MonoBehaviour
{
    [SerializeField] private StringTableCollection tables;

    public void SetData(UIDocument doc)
    {
        var locale = LocalizationSettings.SelectedLocale;
        Debug.Log($"SetData: {locale.Identifier}");
        var table = (StringTable)tables.GetTable(locale.Identifier);

        foreach (var entry in table.Values)
        {
            var el = doc.rootVisualElement.Q(entry.Key);
            switch (el)
            {
                case Label label:
                    label.text = entry.LocalizedValue;
                    Debug.Log($"SetData: {entry.Key} = {entry.LocalizedValue}");
                    break;
                case Button button:
                    button.text = entry.LocalizedValue;
                    Debug.Log($"SetData: {entry.Key} = {entry.LocalizedValue}");
                    break;
                case TextField textField:
                    //textField. = entry.LocalizedValue;
                    break;
                default:
                    Debug.LogWarning($"SetData: {entry.Key} not found");
                    break;
            }
        }
    }
}
