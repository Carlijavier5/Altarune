using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.Events;
using Miniboss;

namespace Miniboss {
    public partial class Miniboss {
        private class PhaseThree : State<MinibossStateInput> {
            // Creating objects
            private Miniboss miniboss;
            private Transform player;
            private NavMeshAgent navigation;
            private Damageable damageable;

            // Creating essential variables
            private float health;
            private float speed;
            private float stoppingDistance;

            public override void Enter(MinibossStateInput input) {
                // Initializes the enemy using the StateInput
                miniboss = input.Miniboss;

                // Initializes objects with values from the enemy
                player = miniboss.player;
                navigation = miniboss.navigation;
                damageable = miniboss.damageable;

                // Pushable implementation
                miniboss.MotionDriver.Set(navigation);


                // Initializes variables with values from the enemy
                speed = miniboss.speed * 2;
                stoppingDistance = 1.5f;
                health = miniboss.health;
            }

            public override void Update(MinibossStateInput input) {
                FollowPlayer();
                health = miniboss.health;
            }

            // Method that outlines how the enemy follows the player
            public void FollowPlayer() {
                // Calculates the normalized vector in the direction of the player
                Vector3 directionToPlayer = (player.position - miniboss.transform.position).normalized;
                // Calculates the angle between the enemy and the player
                float angleToPlayer = Vector3.Angle(miniboss.transform.forward, directionToPlayer);

                // If the angle is less then 60 degrees
                if (angleToPlayer <= 60.0f) {
                    navigation.isStopped = false;
                    navigation.speed = speed;
                    navigation.stoppingDistance = stoppingDistance;
                    navigation.autoBraking = true;
                    navigation.SetDestination(player.position);

                    // Logic to ensure the enemy does not move around if the player moves nearby
                    if ((navigation.remainingDistance <= navigation.stoppingDistance) && !navigation.pathPending) {
                        navigation.isStopped = true;
                    } else {
                        navigation.isStopped = false;
                    }
                } else {
                    navigation.isStopped = true;
                    LookTowardsPlayer();
                }
            }

            public void LookTowardsPlayer() {
                // Determines how the enemy should rotate towards the player
                Vector3 directionToPlayer = (player.position - miniboss.transform.position).normalized;
                Quaternion rotateToPlayer = Quaternion.LookRotation(directionToPlayer);
                // Rotates the enemy towards the player (uses slerp)
                miniboss.transform.rotation = Quaternion.Slerp(miniboss.transform.rotation, rotateToPlayer, 2f * Time.deltaTime);
            }

            // Method to try to damage non-hostile factions
            public void OnTriggerEnter(Collider other) {
                if (other.TryGetComponent(out Entity entity)
                    && entity.Faction != EntityFaction.Hostile) {
                    bool isDamageable = entity.TryDamage(3);
                }
            }

            public override void Exit(MinibossStateInput input) {
                
            }
        }
    }
}