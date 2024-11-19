using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Scaramite {

    [Header("Chase Variables")]
    [SerializeField] private float chaseSpeed = 5f;

    public class Scaramite_Chase : State<Scaramite_Input> {

        public override void Enter(Scaramite_Input input) {
            input.scaramite.driver.SetMaxSpeed(input.scaramite.chaseSpeed);
        }

        public override void Update(Scaramite_Input input) {
            input.scaramite.driver.ResolveRotation();
        }

        public override void FixedUpdate(Scaramite_Input input) {
            Scaramite sm = input.scaramite;

            if (input.aggroTarget != null) {
                Vector3 targetPos = input.aggroTarget.transform.position;
                Vector3 dir = targetPos - sm.transform.position;
                sm.driver.Move(dir);
            } else {
                sm.UpdateAggro();
            }
        }

        public override void Exit(Scaramite_Input input) { }
    }
}