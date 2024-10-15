[System.Serializable]
public class IFrameProperties {

    public DefaultIFrameSettings settings;
    public float duration;

    public IFrameProperties Clone() => MemberwiseClone() as IFrameProperties;
}