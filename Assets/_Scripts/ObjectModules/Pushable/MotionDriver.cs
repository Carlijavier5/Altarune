using UnityEngine;
using UnityEngine.AI;

public enum MotionMode { Transform, Rigidbody, Controller, NavMesh }

[System.Serializable]
public class MotionDriver {

    public event System.Action OnModeChange;

    [HideInInspector][SerializeField] private MotionMode motionMode;
    public MotionMode MotionMode {
        get => motionMode;
        private set {
            OnModeChange?.Invoke();
            motionMode = value;
        }
    }

    [SerializeField] private Transform transform;
    public Transform Transform => transform;

    [SerializeField] private Rigidbody rigidbody;
    public Rigidbody Rigidbody => rigidbody;

    [SerializeField] private CharacterController controller;
    public CharacterController Controller => controller;
    
    [SerializeField] private NavMeshAgent navMeshAgent;
    public NavMeshAgent NavMeshAgent => navMeshAgent;

    public void Set(Transform transform) {
        this.transform = transform;
        MotionMode = MotionMode.Transform;
    }

    public void Set(Rigidbody rigidbody) {
        this.rigidbody = rigidbody;
        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        MotionMode = MotionMode.Rigidbody;
    }

    public void Set(CharacterController controller) {
        this.controller = controller;
        MotionMode = MotionMode.Controller;
    }

    public void Set(NavMeshAgent navMeshAgent) {
        this.navMeshAgent = navMeshAgent;
        MotionMode = MotionMode.NavMesh;
    }

    #if UNITY_EDITOR
    public void EDITOR_ONLY_SetModeDirect(MotionMode motionMode) => MotionMode = motionMode;
    public void EDITOR_ONLY_NullMotionField(MotionMode motionMode) {
        switch (motionMode) {
            case MotionMode.Transform:
                transform = null;
                break;
            case MotionMode.Rigidbody:
                rigidbody = null;
                break;
            case MotionMode.Controller:
                controller = null;
                break;
            case MotionMode.NavMesh:
                navMeshAgent = null;
                break;
        }
    }
    #endif
}