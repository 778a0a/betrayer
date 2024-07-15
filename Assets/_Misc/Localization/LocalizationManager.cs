using System;
using System.Collections.Generic;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;

public class LocalizationManager : MonoBehaviour
{
    [SerializeField] private StringTableCollection tables;

    private readonly List<object> components = new();


    public void Register(object component)
    {
        components.Add(component);
    }

    public void Apply()
    {
        var locale = LocalizationSettings.SelectedLocale;
        var table = (StringTable)tables.GetTable(locale.Identifier);

        foreach (var c in components)
        {
            var typeName = c.GetType().Name;
            var props = c.GetType().GetProperties();
            foreach (var prop in props)
            {
                var entry = table[prop.Name];
                if (entry == null)
                {
                    //Debug.Log($"[L.Apply] Key not found: {prop.Name} of {typeName}");
                    continue;
                }
                var value = entry.LocalizedValue;
                var propValue = prop.GetValue(c);
                switch (propValue)
                {
                    case string:
                        prop.SetValue(c, value);
                        //Debug.Log($"[L.Apply] {entry.Key} = {entry.LocalizedValue}");
                        break;
                    case Label label:
                        label.text = entry.LocalizedValue;
                        //Debug.Log($"[L.Apply] {entry.Key} = {entry.LocalizedValue}");
                        break;
                    case Button button:
                        button.text = entry.LocalizedValue;
                        //Debug.Log($"[L.Apply] {entry.Key} = {entry.LocalizedValue}");
                        break;
                    case TextField textField:
                        //textField. = entry.LocalizedValue;
                        break;
                    default:
                        Debug.LogWarning($"[L.Apply] {entry.Key} is unknown type: {propValue.GetType()}");
                        break;
                }
            }
        }
    }



    public string this[string key, params object[] args] => T(key, args);
    public string T(string key, params object[] args)
    {
        key = key.Replace("\n", "\\n");

        var locale = LocalizationSettings.SelectedLocale;
        var table = (StringTable)tables.GetTable(locale.Identifier);
        var entry = table[key];
        if (entry == null)
        {
            Debug.LogWarning($"Key not found: {key}");
            return string.Format(key, args);
        }
        var value = table[key].LocalizedValue;
        value = value.Replace("\\n", "\n");
        return string.Format(value, args);
    }
}
