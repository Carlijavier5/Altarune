using UnityEngine;
using UnityEditor;

namespace FeatureSamples {
    [CustomEditor(typeof(SamplePushableEntity))]
    public class SamplePushableEntityEditor : Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            SamplePushableEntity entity = target as SamplePushableEntity;

            if (GUILayout.Button("Set Transform Driver")) entity.SetTransformDriver();

            if (GUILayout.Button("Set Kinematic RB Driver")) entity.SetRigidbodyDriver(true);

            if (GUILayout.Button("Set Dynamic RB Driver")) entity.SetRigidbodyDriver(false);

            if (GUILayout.Button("Set Controller Driver")) entity.SetControllerDriver();

            if (GUILayout.Button("Set NavMesh Driver")) entity.SetNavMeshDriver();
        }
    }
}