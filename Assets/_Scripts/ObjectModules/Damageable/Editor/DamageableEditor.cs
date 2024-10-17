using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Damageable))]
public class DamageableEditor : Editor {

    private Damageable Damageable => target as Damageable;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (Application.isPlaying) {
            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.Label("Runtime Health: ");
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                    GUILayout.Label($"{Damageable.Health}");
                }
            }
        }
    }
}