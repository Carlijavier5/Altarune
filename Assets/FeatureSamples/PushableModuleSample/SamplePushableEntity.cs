using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace FeatureSamples {
    public class SamplePushableEntity : Entity {
    
        [System.Serializable]
        private class DriverVars {
            public Transform transform;
            public Rigidbody rigidbody;
            public CharacterController controller;
            public NavMeshAgent navMeshAgent;
        } [SerializeField] private DriverVars driverVariables;

        public void SetTransformDriver() {
            driverVariables.rigidbody.isKinematic = true;
            driverVariables.controller.enabled = false;
            driverVariables.navMeshAgent.enabled = false;
            MotionDriver.Set(driverVariables.transform);
        }

        public void SetRigidbodyDriver(bool isKinematic) {
            driverVariables.rigidbody.isKinematic = isKinematic;
            if (!isKinematic) driverVariables.rigidbody.velocity = Vector3.zero;
            driverVariables.controller.enabled = false;
            driverVariables.navMeshAgent.enabled = false;
            MotionDriver.Set(driverVariables.rigidbody);
        }

        public void SetControllerDriver() {
            driverVariables.rigidbody.isKinematic = true;
            driverVariables.controller.enabled = true;
            driverVariables.navMeshAgent.enabled = false;
            MotionDriver.Set(driverVariables.controller);
        }

        public void SetNavMeshDriver() {
            driverVariables.rigidbody.isKinematic = true;
            driverVariables.controller.enabled = false;
            driverVariables.navMeshAgent.enabled = true;
            MotionDriver.Set(driverVariables.navMeshAgent);
        }

        public void TeleportTo(Vector3 position) {
            bool cEnabled = driverVariables.controller.enabled;
            bool aEnabled = driverVariables.navMeshAgent.enabled;
            driverVariables.controller.enabled = false;
            driverVariables.navMeshAgent.enabled = false;
            if (aEnabled) {
                StartCoroutine(TeleportNavMesh(position));
            } else {
                transform.position = position;
                driverVariables.controller.enabled = cEnabled;
                driverVariables.navMeshAgent.enabled = aEnabled;
            }
        }

        private IEnumerator TeleportNavMesh(Vector3 position) {
            driverVariables.navMeshAgent.Warp(position);
            driverVariables.navMeshAgent.enabled = false;
            yield return new WaitForSeconds(0.05f);
            driverVariables.navMeshAgent.enabled = true;
        }
    }
}