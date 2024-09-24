using UnityEngine;
using UnityEngine.AI;

public enum MotionMode { Transform, Rigidbody, Controller, NavMesh }

[System.Serializable]
public class MotionDriver {

    public MotionMode MotionMode { get; private set; }

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
}