using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

partial class CommonActions
{
    /// <summary>
    /// システムメニューを表示します。
    /// </summary>
    public ShowSystemMenuAction ShowSystemMenu { get; } = new();
    public class ShowSystemMenuAction : CommonActionBase
    {
        public override async ValueTask Do(Character chara)
        {
            var state = SavedGameCoreState.Create(Core);
            var saveData = SaveData.SerializeSaveData(World, state);

            // PlayerPrefsにセーブ
            SaveData.SaveToPlayerPref(saveData);

            // クリップボードにコピー
            GUIUtility.systemCopyBuffer = saveData;
            Debug.Log("セーブしました！");
            Debug.Log(saveData);
        }
    }
}
