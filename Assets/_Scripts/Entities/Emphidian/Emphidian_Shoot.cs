using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Emphidian {

    private const string SHOOT_PARAM = "Shoot";

    [Header("Shoot Variables")]
    [SerializeField] private AnimationClip shootClip;
    [SerializeField] private EmphidianProjectile projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootInterval = 3.7f;
    private Vector3 lockedShootDirection;

    public class State_Shoot : State<Emphidian_Input> {

        private float timer;

        public override void Enter(Emphidian_Input input) {
            Emphidian emp = input.emphidian;
            emp.navMeshAgent.ResetPath();
            if (input.aggroTarget) {
                emp.lockedShootDirection = input.aggroTarget.transform.position
                                         - emp.transform.position;
                emp.animator.SetTrigger(SHOOT_PARAM);
            } else input.stateMachine.SetState(new State_Roam());
            timer = emp.shootClip.length;
        }

        public override void Update(Emphidian_Input input) {
            timer -= input.emphidian.DeltaTime;
            if (timer <= 0) input.stateMachine.SetState(new State_Roam());
        }

        public override void Exit(Emphidian_Input _) { }
    }
}