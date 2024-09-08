#if UNITY_EDITOR
using UnityEngine;

public class DefaultAttributeCurves : ScriptableObject {
    [SerializeField] private AttributeCurves data;
    public AttributeCurves Data => data.Clone();
}
#endif