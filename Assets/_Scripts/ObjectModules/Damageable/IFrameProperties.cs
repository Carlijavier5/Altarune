using UnityEngine;

[System.Serializable]
public class IFrameProperties {

    public Material flashMaterial;
    public float duration;

    public IFrameProperties Clone() => MemberwiseClone() as IFrameProperties;
}