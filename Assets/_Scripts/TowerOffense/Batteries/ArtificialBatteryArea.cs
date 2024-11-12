using UnityEngine;

public class ArtificialBatteryArea : BatteryArea {

    [SerializeField] private ArtificialBattery battery;
    public override IBattery Battery => battery;
}