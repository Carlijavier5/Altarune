using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaShield : BaseObject {

    [SerializeField] private ManaShieldController controller;
    
    void Awake() {
        OnDamageReceived += ManaShield_OnDamageReceived;
        controller.onBreak += Controller_onBreak;
    }

    public void Activate() {
        controller.ShieldSpawn();
        gameObject.SetActive(true);
    } 

    private void ManaShield_OnDamageReceived(int amount) {
        controller.SimulateHit();
    }

    private void Controller_onBreak() {
        gameObject.SetActive(false);
    }

    public override void Perish(bool immediate) {
        base.Perish(immediate);
        controller.ShieldBreak();
    }
}
