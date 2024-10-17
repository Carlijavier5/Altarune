public partial class Player {
    [System.Serializable]
    private class LocomotionProperties {
        public float linearAcceleration, linearDrag,
                     angularSpeed, maxSpeed;

        public LocomotionProperties Clone() => MemberwiseClone() as LocomotionProperties;
    }
}
