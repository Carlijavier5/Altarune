#if UNITY_EDITOR
using UnityEngine;

public class DefaultAttributeCurves : ScriptableObject {
    [SerializeField] private AttributeCurves defaultCurves;
    public AttributeCurves DefaultCurves => defaultCurves.Clone();
}
#endif