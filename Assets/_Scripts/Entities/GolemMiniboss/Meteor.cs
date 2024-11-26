using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GolemSavage {    
    public class Meteor : Entity {
        // Initializing vertical travel variables
        private float groundRiseSpeed = 0.5f;
        private float maxFlightHeight = 5.5f;
        private float flightAcceleration = 7f;
        private float flightDeceleration = 7f;

        private bool riseFirst = false;
        private GameObject shadow;

        // Initializing coroutines
        private Coroutine riseFromGround;
        private Coroutine moveInwards;
        private Coroutine shootUp;
        private Coroutine fly;
        private Coroutine fall;

        // Initializing events
        public UnityEvent onMeteorDestruction;

        void Start() {
            SetMeteorSize();
        }

        public void InitializeMeteor(bool riseFirst) {
            this.riseFirst = riseFirst;

            if (riseFirst) {
                riseFromGround = StartCoroutine(RiseFromGroundCoroutine());
            } else {
                SetMeteorPosition();
                fall = StartCoroutine(FallCoroutine());
            }
        }

        private void SetMeteorSize() {
            // Min and Max scale values
            float minScaleX = 1f;
            float maxScaleX = 1.5f;

            float minScaleY = 1f;
            float maxScaleY = 1.5f;

            float minScaleZ = 1f;
            float maxScaleZ = 1.5f;

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
            moveInwards = StartCoroutine(MoveInwards());
        }

        private IEnumerator MoveInwards() {
            float initialRadius = 8f;

            // Duration of animation
            float duration = 5f;
            float timeElapsed = 0f;

            Vector3 currentPosition = transform.position;
            float inwardsAcceleration = 2f; 

            while (timeElapsed < duration) {
                float progress = timeElapsed / duration;

                // Decreasing the radius
                float currentRadius = Mathf.Lerp(initialRadius, 1f, progress);

                // Calculating the new angle and positions
                float angle = Mathf.Atan2(currentPosition.z, currentPosition.x);
                float positionX = Mathf.Cos(angle) * currentRadius;
                float positionZ = Mathf.Sin(angle) * currentRadius;

                transform.position = new Vector3(positionX, currentPosition.y, positionZ);
                inwardsAcceleration = Mathf.Lerp(1f, 5f, progress);

                timeElapsed += Time.deltaTime * inwardsAcceleration;

                yield return null;
            }
            shootUp = StartCoroutine(ShootUp());
        }

        private IEnumerator ShootUp() {
            float currentVelocity = 30f;

            yield return new WaitForSeconds(0.5f);
            while (transform.position.y < 100f) {
                // Increments the current velocity, and increases the height
                currentVelocity += flightAcceleration * Time.deltaTime;
                transform.position += Vector3.up * currentVelocity * Time.deltaTime;

                yield return null;
            }
            Perish();
        }

        private IEnumerator FallCoroutine() {
            float currentVelocity = 0f;

            // Gets the shadow (child of meteor)
            Transform shadowTransform = transform.Find("Shadow");

            while (transform.position.y > 0.5) {
                // Decrements the current velocity, and decreases the height
                currentVelocity += flightDeceleration * Time.deltaTime;
                transform.position -= Vector3.up * currentVelocity * Time.deltaTime;

                // Makes the shadow stay on the ground
                shadowTransform.position = new Vector3(transform.position.x, 0.3f, transform.position.z);

                // Scales the shadow based on the y position
                float scaleFactor = Mathf.Lerp(0.5f, 2f, (100f - transform.position.y) / 100f);
                shadowTransform.localScale = new Vector3(scaleFactor, 1f, scaleFactor);
                
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
            if (riseFromGround != null) StopCoroutine(riseFromGround);
            if (moveInwards != null) StopCoroutine(moveInwards);
            if (shootUp != null) StopCoroutine(shootUp);
            if (fly != null) StopCoroutine(fly);
            if (fall != null) StopCoroutine(fall);
        }
    }
}

