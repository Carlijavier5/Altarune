using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LoopingSystemController))]
public class LoopingSystemControllerEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUILayout.GetControlRect(false, 1);
        CJUtils.GUIUtils.DrawSeparatorLine();
        EditorGUILayout.GetControlRect(false, 1);
        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
            GUI.color = CJUtils.UIColors.Green;
            if (GUILayout.Button("Enable")) {
                (target as LoopingSystemController).Enable();
            } GUI.color = CJUtils.UIColors.Red;
            if (GUILayout.Button("Disable")) {
                (target as LoopingSystemController).Disable();
            } GUI.color = Color.white;
        }
    }
}