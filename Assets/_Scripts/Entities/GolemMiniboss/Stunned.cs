using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.Events;
using Miniboss;

namespace Miniboss {
    public partial class Miniboss {
        private class Stunned : State<MinibossStateInput> {
            // Creating objects
            private Miniboss miniboss;
            private NavMeshAgent navigation;
            private Damageable damageable;

            private float health;
           
            public override void Enter(MinibossStateInput input) {
                // Initializes the enemy using the StateInput
                miniboss = input.Miniboss;

                // Initializes objects with values from the enemy
                navigation = miniboss.navigation;
                miniboss.MotionDriver.Set(navigation);
                damageable = miniboss.damageable;

                health = miniboss.health;
            }

            public override void Update(MinibossStateInput input) {   
                
            }

            // Method to try to damage non-hostile factions
            public void OnTriggerEnter(Collider other) {
                if (other.TryGetComponent(out Entity entity)
                    && entity.Faction != EntityFaction.Hostile) {
                    bool isDamageable = entity.TryDamage(3);
                }
            }

            private void CheckStateTransition() {
               
            }

            public override void Exit(MinibossStateInput input) {
                
            }
        }
    }
}