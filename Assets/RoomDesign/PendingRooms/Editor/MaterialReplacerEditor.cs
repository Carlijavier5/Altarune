using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MaterialReplacer))]
public class MaterialReplacerEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUILayout.GetControlRect(false, 1);
        CJUtils.GUIUtils.DrawSeparatorLine();
        EditorGUILayout.GetControlRect(false, 1);
        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
            GUI.color = CJUtils.UIColors.Blue;
            if (GUILayout.Button("Replace Material(s)")) {
                (target as MaterialReplacer).ReplaceMaterialsInRenderers();
            }
            GUI.color = Color.white;
        }
    }
}