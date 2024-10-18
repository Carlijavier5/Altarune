#if UNITY_EDITOR
using UnityEngine;

public class DefaultIFrameProperties : ScriptableObject {
    [SerializeField] private IFrameProperties defaultProperties;
    public IFrameProperties DefaultProperties => defaultProperties.Clone();
}
#endif