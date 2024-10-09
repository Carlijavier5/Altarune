using System.Collections.Generic;
using UnityEngine;

namespace FeatureSamples {
    public class SamplePullSource : MonoBehaviour {

        [SerializeField] private float pullStrength;
        private readonly HashSet<BaseObject> baseObjects = new();
        private readonly Stack<BaseObject> terminateStack = new();

        void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out BaseObject baseObject)) {
                baseObjects.Add(baseObject);
            }
        }

        private void FixedUpdate() {
            foreach (BaseObject baseObject in baseObjects) {
                if (baseObject) {
                    baseObject.TryPush(ComputeXZDirection(baseObject.transform, transform), pullStrength);
                } else terminateStack.Push(baseObject);
            }

            while (terminateStack.TryPop(out BaseObject baseObject)) baseObjects.Remove(baseObject);
        }

        /// <summary>
        /// Applies a long push to each object in range; <br/>
        /// Sets the ease curve of each push to logarithmic;
        /// </summary>
        public void ApplyLongPush(float strength, float duration) {
            foreach (BaseObject baseObject in baseObjects) {
                baseObject.TryLongPush(ComputeXZDirection(baseObject.transform, transform),
                                       strength, duration, out PushActionCore core);
                core.SetEase(EaseCurve.Logarithmic);
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