using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Emphidian {

    [Header("Roam Variables")]
    [SerializeField] private Vector2 moveTimeRange = new(3f, 4.5f);
    [SerializeField] private Vector2 roamDistanceRange;
    [SerializeField]
    private float roamSpeed = 3.5f,
                                   stoppingDistance = 0.1f,
                                   wiggleAmplitude = 0.5f,
                                   wiggleFrequency = 2.0f,
                                   maxWiggleDistance;
    [Range(0f, 1f)]
    [Tooltip("Bias toward the player;\n"
                          + "• 0 - Do not account for the player;\n"
                          + "• 1 - Move only towards the player;")]
    [SerializeField] private float playerBias = 0.65f;

    public class State_Roam : State<Emphidian_Input> {

        private Emphidian emp;
        private float moveTime, moveTimer,
                      shootTimer;

        public override void Enter(Emphidian_Input input) {
            emp = input.emphidian;
            emp.navMeshAgent.SetDestination(emp.transform.position);
            moveTime = Random.Range(emp.moveTimeRange.x, emp.moveTimeRange.y);
        }

        public override void Update(Emphidian_Input input) {
            moveTimer += emp.DeltaTime;

            float distance = emp.navMeshAgent.remainingDistance;
            if (distance > emp.stoppingDistance) {
                Vector3 dir = emp.navMeshAgent.destination - emp.transform.position;
                Vector2 offset = Mathf.Sin(Time.time * emp.wiggleFrequency)
                               * Mathf.Clamp(distance / emp.maxWiggleDistance, 0,
                                             emp.maxWiggleDistance)
                               * emp.wiggleAmplitude * emp.DeltaTime
                               * new Vector2(-dir.z, dir.x);
                emp.navMeshAgent.Move(new Vector3(offset.x, 0, offset.y));
            } else if (input.aggroTarget) {
                Vector3 dir = input.aggroTarget.transform.position - emp.transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                emp.transform.rotation = Quaternion.RotateTowards(emp.transform.rotation,
                        targetRotation, emp.DeltaTime * emp.navMeshAgent.angularSpeed);
            }

            if (moveTimer >= moveTime) {
                Vector3 destination = emp.GetRoamDestination();
                emp.navMeshAgent.SetDestination(destination);
                moveTime = Random.Range(emp.moveTimeRange.x, emp.moveTimeRange.y);
                moveTimer = 0;
            }

            shootTimer += emp.DeltaTime;
            if (shootTimer >= emp.shootInterval && input.aggroTarget) {
                input.stateMachine.SetState(new State_Shoot());
            }
        }

        public override void Exit(Emphidian_Input _) { }
    }
}