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

public partial class MessageWindow
{
    public Button buttonMessageOK { get; private set; }

    public Button buttonMessageYes { get; private set; }

    public Button buttonMessageNo { get; private set; }

    public Button buttonMessageCancel { get; private set; }

    public Label labelMessageWindowTitle { get; private set; }

    public Label labelMessageText { get; private set; }

    public VisualElement Root { get; }

    public MessageWindow(VisualElement root)
    {
        Root = root;
        buttonMessageOK = Root?.Q<Button>("buttonMessageOK");
        buttonMessageYes = Root?.Q<Button>("buttonMessageYes");
        buttonMessageNo = Root?.Q<Button>("buttonMessageNo");
        buttonMessageCancel = Root?.Q<Button>("buttonMessageCancel");
        labelMessageWindowTitle = Root?.Q<Label>("labelMessageWindowTitle");
        labelMessageText = Root?.Q<Label>("labelMessageText");
    }
}