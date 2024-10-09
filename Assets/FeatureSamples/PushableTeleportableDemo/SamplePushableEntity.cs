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
    }
}