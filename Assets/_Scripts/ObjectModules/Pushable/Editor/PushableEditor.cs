using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

[CustomEditor(typeof(Pushable))]
public class PushableEditor : Editor {

    private Pushable Module => target as Pushable;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        serializedObject.Update();

        MotionDriver driver = Module.EDITOR_ONLY_DefaultDriver;
        MotionMode newMode = (MotionMode) EditorGUILayout.EnumPopup("Default Motion Mode", driver.MotionMode);
        if (newMode != driver.MotionMode) UpdateLocalDriver(newMode);

        switch (driver.MotionMode) {
            case MotionMode.Transform:
                Transform transform = EditorGUILayout.ObjectField("Transform", driver.Transform,
                                                    typeof(Transform), true) as Transform;
                if (transform != driver.Transform) {
                    if (transform == null) driver.EDITOR_ONLY_NullMotionField(MotionMode.Transform);
                    else driver.Set(transform);
                } break;
            case MotionMode.Rigidbody:
                Rigidbody rigidbody = EditorGUILayout.ObjectField("Rigidbody", driver.Rigidbody,
                                                    typeof(Rigidbody), true) as Rigidbody;
                if (rigidbody != driver.Rigidbody) {
                    if (rigidbody == null) driver.EDITOR_ONLY_NullMotionField(MotionMode.Rigidbody);
                    else driver.Set(rigidbody);
                } break;
            case MotionMode.Controller:
                CharacterController controller = EditorGUILayout.ObjectField("Character Controller", driver.Controller,
                                                    typeof(CharacterController), true) as CharacterController;
                if (controller != driver.Controller) {
                    if (controller == null) driver.EDITOR_ONLY_NullMotionField(MotionMode.Controller);
                    else driver.Set(controller);
                } break;
            case MotionMode.NavMesh:
                NavMeshAgent agent = EditorGUILayout.ObjectField("NavMesh Agent", driver.NavMeshAgent,
                                                    typeof(NavMeshAgent), true) as NavMeshAgent;
                if (agent != driver.NavMeshAgent) {
                    if (agent == null) driver.EDITOR_ONLY_NullMotionField(MotionMode.NavMesh);
                    else driver.Set(agent);
                } break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void UpdateLocalDriver(MotionMode newMode) {
        MotionDriver driver = Module.EDITOR_ONLY_DefaultDriver;
        switch (newMode) {
            case MotionMode.Transform:
                if (driver.Transform == null) {
                    driver.Set(Module.transform);
                } else {
                    driver.EDITOR_ONLY_SetModeDirect(MotionMode.Transform);
                } break;
            case MotionMode.Rigidbody:
                if (driver.Rigidbody == null
                    && Module.TryGetComponent(out Rigidbody rigidbody)) {
                        driver.Set(rigidbody);
                } else {
                    driver.EDITOR_ONLY_SetModeDirect(MotionMode.Rigidbody);
                } break;
            case MotionMode.Controller:
                if (driver.Controller == null 
                    && Module.TryGetComponent(out CharacterController controller)) {
                    driver.Set(controller);
                } else {
                    driver.EDITOR_ONLY_SetModeDirect(MotionMode.Controller);
                } break;
            case MotionMode.NavMesh:
                if (driver.NavMeshAgent == null
                    && Module.TryGetComponent(out NavMeshAgent agent)) {
                    driver.Set(agent);
                } else {
                    driver.EDITOR_ONLY_SetModeDirect(MotionMode.NavMesh);
                } break;
        }
    }
}
