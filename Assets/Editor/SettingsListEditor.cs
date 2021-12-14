using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

[CustomEditor(typeof(SettingsList))]
public class SettingsListEditor : Editor
{
    public override void OnInspectorGUI() {
        SettingsList settingsList = target as SettingsList;

        settingsList.type = (SettingsList.SettingsType)EditorGUILayout.EnumPopup("type", settingsList.type);

        if (settingsList.type == SettingsList.SettingsType.LIST) {
            settingsList.itemName = (TextMeshProUGUI)EditorGUILayout.ObjectField("itemName", settingsList.itemName, typeof(TextMeshProUGUI));

            settingsList.rightButton = (Button)EditorGUILayout.ObjectField("rightButton", settingsList.rightButton, typeof(Button));

            settingsList.leftButton = (Button)EditorGUILayout.ObjectField("leftButton", settingsList.leftButton, typeof(Button));
        }
        else {
            settingsList.slider = (Slider)EditorGUILayout.ObjectField("slider", settingsList.slider, typeof(Slider));
        }
        EditorUtility.SetDirty(target);
    }
}
