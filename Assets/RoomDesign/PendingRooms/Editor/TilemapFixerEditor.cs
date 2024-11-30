using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TilemapFixer))]
public class TilemapFixerEditor : Editor {

    private Vector3 offset;

    public override void OnInspectorGUI() {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
            offset = EditorGUILayout.Vector3Field("Offset", offset);
            if (GUILayout.Button("Displace Tiles")) {
                (target as TilemapFixer).DisplaceTiles(offset);
            }
        }
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
            if (GUILayout.Button("Unparent Children")) {
                (target as TilemapFixer).UnparentChildren();
            }
        }
    }
}