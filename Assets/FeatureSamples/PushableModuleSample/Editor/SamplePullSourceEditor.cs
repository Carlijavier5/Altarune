using UnityEngine;
using UnityEditor;

namespace FeatureSamples {
    [CustomEditor(typeof(SamplePullSource))]
    public class SamplePullSourceEditor : Editor {

        private float strength = 4;
        private float duration = 2;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            SamplePullSource pullSource = target as SamplePullSource;

            EditorGUILayout.GetControlRect(false, 2);
            CJUtils.GUIUtils.DrawSeparatorLine();
            EditorGUILayout.GetControlRect(false, 2);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                if (GUILayout.Button("Apply Long Push")) {
                    pullSource.ApplyLongPush(strength, duration);
                }
                using (new EditorGUILayout.HorizontalScope()) {
                    EditorGUIUtility.labelWidth = 60;
                    strength = EditorGUILayout.FloatField("Strength", strength);
                    GUILayout.Space(2); GUILayout.Label("|", GUILayout.Width(8)); GUILayout.Space(2);
                    duration = EditorGUILayout.FloatField("Duration", duration);
                    EditorGUIUtility.labelWidth = 0;
                }
            }
        }
    }
}