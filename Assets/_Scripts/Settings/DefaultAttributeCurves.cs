#if UNITY_EDITOR
using UnityEngine;

public class DefaultAttributeCurves : ScriptableObject {
    [SerializeField] private HealthAttributeCurves defaultCurves;
    public HealthAttributeCurves DefaultCurves => defaultCurves.Clone();
}
#endif