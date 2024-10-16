using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

[CustomEditor(typeof(Pushable))]
public class PushableEditor : Editor {

    private Pushable Module => target as Pushable;
    private BaseObject baseObject;

    private const string UNDO_MESSAGE = "Change pushable property";

    void Awake() {
        SerializedProperty boProperty = serializedObject.FindProperty("baseObject");
        if (boProperty != null) baseObject = boProperty.objectReferenceValue as BaseObject;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        MotionDriver driver = Module.EDITOR_ONLY_DefaultDriver;
        MotionMode newMode = (MotionMode) EditorGUILayout.EnumPopup("Default Motion Mode", driver.MotionMode);
        if (newMode != driver.MotionMode) {
            Undo.RecordObject(target, UNDO_MESSAGE);
            UpdateLocalDriver(newMode);
            PrefabUtility.RecordPrefabInstancePropertyModifications(target);
        }

        switch (driver.MotionMode) {
            case MotionMode.Transform:
                Transform transform = EditorGUILayout.ObjectField("Transform", driver.Transform,
                                                    typeof(Transform), true) as Transform;
                if (transform != driver.Transform) {
                    Undo.RecordObject(target, UNDO_MESSAGE);
                    if (transform == null) driver.EDITOR_ONLY_NullMotionField(MotionMode.Transform);
                    else driver.Set(transform);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                } break;
            case MotionMode.Rigidbody:
                Rigidbody rigidbody = EditorGUILayout.ObjectField("Rigidbody", driver.Rigidbody,
                                                    typeof(Rigidbody), true) as Rigidbody;
                if (rigidbody != driver.Rigidbody) {
                    Undo.RecordObject(target, UNDO_MESSAGE);
                    if (rigidbody == null) driver.EDITOR_ONLY_NullMotionField(MotionMode.Rigidbody);
                    else driver.Set(rigidbody);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                } break;
            case MotionMode.Controller:
                CharacterController controller = EditorGUILayout.ObjectField("Character Controller", driver.Controller,
                                                    typeof(CharacterController), true) as CharacterController;
                if (controller != driver.Controller) {
                    Undo.RecordObject(target, UNDO_MESSAGE);
                    if (controller == null) driver.EDITOR_ONLY_NullMotionField(MotionMode.Controller);
                    else driver.Set(controller);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                } break;
            case MotionMode.NavMesh:
                NavMeshAgent agent = EditorGUILayout.ObjectField("NavMesh Agent", driver.NavMeshAgent,
                                                    typeof(NavMeshAgent), true) as NavMeshAgent;
                if (agent != driver.NavMeshAgent) {
                    Undo.RecordObject(target, UNDO_MESSAGE);
                    if (agent == null) driver.EDITOR_ONLY_NullMotionField(MotionMode.NavMesh);
                    else driver.Set(agent);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                } break;
        }

        if (Application.isPlaying && baseObject != null) {
            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.Label("Runtime Driver:");
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                    GUILayout.Label($"{baseObject.MotionDriver.MotionMode}");
                }
            }
        }
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