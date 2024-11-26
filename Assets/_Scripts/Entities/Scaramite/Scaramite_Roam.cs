using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Scaramite {

    [Header("Roam Variables")]
    [SerializeField] private Vector2 moveTimeRange = new(3f, 4.5f);
    [SerializeField] private float roamSpeed = 3.5f;
    [SerializeField] private float minRoamDistance;
    [SerializeField] private float maxRoamDistance;
    [SerializeField] private float stoppingDistance = 0.1f;
    [Range(0f, 1f)][Tooltip("Bias toward the player;\n"
                          + "• 0 - Do not account for the player;\n"
                          + "• 1 - Move only towards the player;")]
    [SerializeField] private float playerBias = 0.65f;

    public class Scaramite_Roam : State<Scaramite_Input> {

        private Vector3 destination;
        private float moveTime;
        private float timer;

        public override void Enter(Scaramite_Input input) {
            Scaramite sm = input.scaramite;
            sm.driver.SetMaxSpeed(input.scaramite.roamSpeed);
            destination = sm.transform.position;
            moveTime = Random.Range(sm.moveTimeRange.x, sm.moveTimeRange.y);
            timer = 0f;
        }

        public override void Update(Scaramite_Input input) {
            input.scaramite.driver.ResolveRotation();
        }

        public override void FixedUpdate(Scaramite_Input input) {
            Scaramite sm = input.scaramite;

            timer += sm.FixedDeltaTime;
            destination.y = sm.transform.position.y;

            if (Vector3.Distance(destination, sm.transform.position) > sm.stoppingDistance) {
                Vector3 dir = destination - sm.transform.position;
                sm.driver.Move(dir);
            } else {
                sm.driver.LookAt(sm.player.transform.position);
            }

            if (timer >= moveTime) {
                destination = sm.GetRoamDestination();
                moveTime = Random.Range(sm.moveTimeRange.x, sm.moveTimeRange.y);
                timer = 0f;
            }
        }

        public override void Exit(Scaramite_Input input) { }
    }
}
