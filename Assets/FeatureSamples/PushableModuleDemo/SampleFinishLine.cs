using UnityEngine;

namespace FeatureSamples {
    public class SampleFinishLine : MonoBehaviour {

        [SerializeField] private SamplePushableEntity sampleEntity;
        private Vector3 ogPos;

        private float localTimer;

        void Awake() => ogPos = sampleEntity.transform.position;

        void Update() => localTimer += Time.deltaTime;

        void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out SamplePushableEntity entity)
                && entity == sampleEntity) {
                sampleEntity.TeleportTo(ogPos);

                Debug.Log($"MotionMode: {sampleEntity.MotionDriver.MotionMode}, Time: {localTimer}");
                localTimer = 0;
            }
        }
    }
}