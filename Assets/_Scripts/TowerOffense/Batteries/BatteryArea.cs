using UnityEngine;

public abstract class BatteryArea : MonoBehaviour {

    public abstract IBattery Battery { get; }
    public bool IsActive => Battery.IsActive;
}
