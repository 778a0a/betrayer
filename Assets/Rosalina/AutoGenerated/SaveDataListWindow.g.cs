//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rosalina Code Generator tool.
//     Version: 4.0.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SaveDataListWindow
{
    public Button CloseButton { get; private set; }

    public SaveDataListWindowListItem SaveSlot1 { get; private set; }

    public SaveDataListWindowListItem SaveSlot2 { get; private set; }

    public SaveDataListWindowListItem SaveSlot3 { get; private set; }

    public SaveDataListWindowListItem SaveSlotAuto { get; private set; }

    public VisualElement Border { get; private set; }

    public VisualElement Root { get; }

    public SaveDataListWindow(VisualElement root)
    {
        Root = root;
        CloseButton = Root?.Q<Button>("CloseButton");
        SaveSlot1 = new SaveDataListWindowListItem(Root?.Q<VisualElement>("SaveSlot1"));
        SaveSlot2 = new SaveDataListWindowListItem(Root?.Q<VisualElement>("SaveSlot2"));
        SaveSlot3 = new SaveDataListWindowListItem(Root?.Q<VisualElement>("SaveSlot3"));
        SaveSlotAuto = new SaveDataListWindowListItem(Root?.Q<VisualElement>("SaveSlotAuto"));
        Border = Root?.Q<VisualElement>("Border");
    }
}