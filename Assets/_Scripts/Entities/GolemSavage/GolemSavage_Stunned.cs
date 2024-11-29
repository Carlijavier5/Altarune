using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.Events;
using GolemSavage;

namespace GolemSavage {
    public partial class GolemSavage {
        private class GolemSavage_Stunned : State<GolemSavageStateInput> {
            // Creating objects
            private GolemSavage golemSavage;
            private NavMeshAgent navigation;
            private Damageable damageable;

            private float health;
           
            public override void Enter(GolemSavageStateInput input) {
                // Initializes the enemy using the StateInput
                golemSavage = input.GolemSavage;

                // Initializes objects with values from the enemy
                navigation = golemSavage.navigation;
                golemSavage.MotionDriver.Set(navigation);
                damageable = golemSavage.damageable;

                health = golemSavage.health;
            }

            public override void Update(GolemSavageStateInput input) {   
                
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

            public override void Exit(GolemSavageStateInput input) {
                
            }
        }
    }
}