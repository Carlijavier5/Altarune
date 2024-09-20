#if UNITY_EDITOR
using UnityEngine;

[CreateAssetMenu()]
public class DefaultIFrameProperties : ScriptableObject {
    [SerializeField] private IFrameProperties defaultProperties;
    public IFrameProperties DefaultProperties => defaultProperties.Clone();
}
#endif