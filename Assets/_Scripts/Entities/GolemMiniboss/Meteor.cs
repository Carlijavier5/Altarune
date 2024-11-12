using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GolemSavage {    
    public class Meteor : Entity {
        // Initializing vertical travel variables
        private float groundRiseSpeed;
        private float maxFlightHeight = 80f;
        private float flightAcceleration = 15f;
        private float flightDeceleration = 10f;

        // Initializing coroutines
        private Coroutine riseFromGround;
        private Coroutine fly;
        private Coroutine fall;

        // Initializing events
        public UnityEvent onMeteorDestruction;

        void Start() {
            SetMeteorSize();

            // Random ground appearance speeds
            groundRiseSpeed = Random.Range(0.2f, 0.3f);

            riseFromGround = StartCoroutine(RiseFromGroundCoroutine());
        }

        private void SetMeteorSize() {
            // Min and Max scale values
            float minScaleX = 1f;
            float maxScaleX = 2f;

            float minScaleY = 1f;
            float maxScaleY = 2f;

            float minScaleZ = 1f;
            float maxScaleZ = 2f;

            // Randomizes scaling of X, Y, and Z
            float scaleX = Random.Range(minScaleX, maxScaleX);
            float scaleY = Random.Range(minScaleY, maxScaleY);
            float scaleZ = Random.Range(minScaleZ, maxScaleZ);

            // Scales the meteors
            transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        }

        private void SetMeteorPosition() {
            // Possible location bounds for spawning
            float minX = -9f;
            float maxX = 9f;
            float minZ = -9f;
            float maxZ = 9f;

            float positionX = Random.Range(minX, maxX);
            float positionZ = Random.Range(minZ, maxZ);

            transform.position = new Vector3(positionX, 100f, positionZ);
        }

        private IEnumerator RiseFromGroundCoroutine() {
            // Waits one second before appearing
            yield return new WaitForSeconds(0.3f);

            // Meteors rise from the ground
            while (transform.position.y < 0) {
                transform.position += Vector3.up * groundRiseSpeed * Time.deltaTime;
                yield return null;
            }

            fly = StartCoroutine(FlyCoroutine());
        }

        private IEnumerator FlyCoroutine() {
            float currentVelocity = 0f;

            while (transform.position.y < maxFlightHeight) {
                // Increments the current velocity, and increases the height
                currentVelocity += flightAcceleration * Time.deltaTime;
                transform.position += Vector3.up * currentVelocity * Time.deltaTime;

                yield return null;
            }

            // Randomizes the positions of the meteors when falling
            SetMeteorPosition();
            fall = StartCoroutine(FallCoroutine());
        }

        private IEnumerator FallCoroutine() {
            float currentVelocity = 0f;

            while (transform.position.y > 0.5) {
                // Decrements the current velocity, and decreases the height
                currentVelocity += flightDeceleration * Time.deltaTime;
                transform.position -= Vector3.up * currentVelocity * Time.deltaTime;

                yield return null;
            }
            Perish();
        }

        // Method to try to damage non-hostile factions
        public void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out Entity entity)
                && entity.Faction != EntityFaction.Hostile) {
                bool isDamageable = entity.TryDamage(3);
            }
        }

        // Method called when enemy loses all its health
        public override void Perish() {
            Exit();

            onMeteorDestruction.Invoke();
            DetachModules();
            enabled = false;
            Destroy(gameObject, 5);
            base.Perish();
        }

        public void Exit() {
            // Stop the Coroutines
            StopCoroutine(riseFromGround);
            StopCoroutine(fly);
            StopCoroutine(fall);
        }
    }
}

