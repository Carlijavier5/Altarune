using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GolemSavage {    
    public class MeteorSpawner : MonoBehaviour {
        // Creating the meteor prefab and array
        private GolemSavage golemSavage;
        [SerializeField] private GameObject meteorPrefab;
        private GameObject[] meteors;

        // Initializing vertical travel variables
        [SerializeField] private int numMeteor = 15;
        [SerializeField] private float radius = 8f;

        // Initializing coroutines
        private Coroutine meteorRise;
        private Coroutine meteorFall;

        public void StartMeteorSpawning() {
            meteorRise = StartCoroutine(MeteorRiseAnimation());
        }

        private IEnumerator MeteorRiseAnimation() {
            SpawnAnimationMeteors();
            yield return new WaitForSeconds(6f);

            meteorFall = StartCoroutine(MeteorFall());
        }

        public void SpawnAnimationMeteors() {
            // Possible location bounds for spawning
            float minX = -10f;
            float maxX = 10f;
            float minZ = -10f;
            float maxZ = 10f;
            float positionY = -1f;

            Vector3[] spawnVectors = new Vector3[numMeteor];
            for (int i = 0; i < numMeteor; i++) {
                    // Randomly generate the spawn location vectors
                    float positionX = Random.Range(minX, maxX);
                    float positionZ = Random.Range(minZ, maxZ);
                    spawnVectors[i] = new Vector3(positionX, positionY, positionZ);
                    
                    // Instantiates the meteors below the ground
                    GameObject meteor = Instantiate(meteorPrefab, spawnVectors[i], Quaternion.identity);
                    
                    // Adds the on destruction listener
                    Meteor meteorController = meteor.GetComponent<Meteor>();
                    meteorController.InitializeMeteor(true);
            }
        }

        private IEnumerator MeteorFall() {
            while (true) {
                yield return new WaitForSeconds(.7f);
                SpawnFallingMeteors();
            }
        }

        public void SpawnFallingMeteors() {
            // Possible location bounds for spawning
            float minX = -10f;
            float maxX = 10f;
            float minZ = -10f;
            float maxZ = 10f;
            float positionY = 80f;

            // Randomly generate the spawn location vectors
            float positionX = Random.Range(minX, maxX);
            float positionZ = Random.Range(minZ, maxZ);
            Vector3 spawnLocation = new Vector3(positionX, positionY, positionZ);

            // Instantiates the meteors below the ground
            GameObject meteor = Instantiate(meteorPrefab, spawnLocation, Quaternion.identity);

            // Adds the on destruction listener
            Meteor meteorController = meteor.GetComponent<Meteor>();
            meteorController.InitializeMeteor(false);
        }

        public void Exit() {
            // Stop the Coroutines
            if (meteorRise != null) StopCoroutine(meteorRise);
            if (meteorFall != null) StopCoroutine(meteorFall);
        }
    }
}

