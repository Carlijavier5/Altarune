using UnityEngine;

[System.Serializable]
public class TeleportableProperties {
    public DefaultTeleportableSettings settings;
    public Transform rootTransform;

    public TeleportableProperties(DefaultTeleportableSettings settings,
                                  Transform rootTransform) {
        this.settings = settings;
        this.rootTransform = rootTransform;
    }
}