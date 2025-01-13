using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Condition class for checking if the player presses a certain key. Combine with a linked condition to check for multiple inputs.
/// </summary>
public class MeleeCondition : CCondition {
    [SerializeField] private int pressesRequired = 1;

    private int presses;

    private void Start() {
        GM.Instance.OnPlayerInit += RegisterEvent;
    }

    private void RegisterEvent() {
        GM.Player.InputSource.OnMeleePerformed += CheckCondition;
    }
    
    protected override void CheckCondition() {
        if (!active) return;
        presses++;
        if (pressesRequired <= presses) {
            SendActivationEvent();
            //reset if reusable
            if (!singleUse) {
                presses = 0;
            }
        }
    }
}
