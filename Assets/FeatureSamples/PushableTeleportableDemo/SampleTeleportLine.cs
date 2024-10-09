using UnityEngine;

namespace FeatureSamples {
    public class SampleTeleportLine : MonoBehaviour {

        [SerializeField] private Transform target;

        private float localTimer;

        void Update() => localTimer += Time.deltaTime;

        void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out BaseObject baseObject)) {
                baseObject.TryTeleport(target.position);

                Debug.Log($"MotionMode: {baseObject.MotionDriver.MotionMode}, Time: {localTimer}");
                localTimer = 0;
            }
        }
    }
}