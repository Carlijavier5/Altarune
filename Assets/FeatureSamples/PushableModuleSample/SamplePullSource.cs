using System.Collections.Generic;
using UnityEngine;

namespace FeatureSamples {
    public class SamplePullSource : MonoBehaviour {

        [SerializeField] private float pullStrength;
        private readonly HashSet<BaseObject> baseObjects = new();

        void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out BaseObject baseObject)) {
                baseObjects.Add(baseObject);
            }
        }

        private void FixedUpdate() {
            foreach (BaseObject baseObject in baseObjects) {
                baseObject.TryPush(ComputeXZDirection(baseObject.transform, transform), pullStrength);
            }
        }

        public void ApplyLongPush(float strength, float duration) {
            foreach (BaseObject baseObject in baseObjects) {
                baseObject.TryLongPush(ComputeXZDirection(baseObject.transform, transform),
                                       strength, duration);
            }
        }

        /// <summary>
        /// Compute XZ direction vector
        /// </summary>
        /// <param name="t1"> From; </param>
        /// <param name="t2"> To; </param>
        private Vector3 ComputeXZDirection(Transform t1, Transform t2) {
            return new Vector3(t2.position.x, 0, t2.position.z) 
                 - new Vector3(t1.position.x, 0, t1.position.z);
        }
    }
}