using UnityEngine;

[System.Serializable]
public class TeleportableProperties {
    public DefaultTeleportableSettings settings;
    public Transform rootTransform;
    public float duration = 0.5f;

    public TeleportableProperties(DefaultTeleportableSettings settings,
                                  Transform rootTransform) {
        this.settings = settings;
        this.rootTransform = rootTransform;
        duration /= 2;
    }
}