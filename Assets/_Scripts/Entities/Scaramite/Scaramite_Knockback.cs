using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Scaramite {

    [Header("Self-Knockback Variables")]
    [SerializeField] private float knockbackForce = 2f;
    [SerializeField] private float knockbackDuration = 0.8f;

    public class Scaramite_Knockback : State<Scaramite_Input> {

        private Vector3 kbDir;
        private readonly float knockbackEndTime;

        public Scaramite_Knockback(Scaramite sm, BaseObject contact) {
            kbDir = (sm.transform.position - contact.transform.position).normalized;
            kbDir.y = 0;
            sm.TryLongPush(kbDir, sm.knockbackForce, sm.knockbackDuration);
            knockbackEndTime = Time.time + sm.knockbackDuration;
        }

        public override void Enter(Scaramite_Input input) { }

        public override void Update(Scaramite_Input input) {
            Scaramite sm = input.scaramite;
            sm.driver.LookAt(sm.transform.position - kbDir);
            sm.driver.ResolveRotation();

            if (Time.time >= knockbackEndTime) {
                input.stateMachine.SetState(new Scaramite_Chase());
            }
        }

        public override void Exit(Scaramite_Input input) { }
    }
}