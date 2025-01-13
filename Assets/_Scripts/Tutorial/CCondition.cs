using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public abstract class CCondition : MonoBehaviour {
    [SerializeField] protected CConditionData data;
    [SerializeField] protected bool singleUse = true;
    [SerializeField] protected bool active = true;

    private bool conditionMet = false;
    
    public delegate void ConditionTrigger(CConditionData data);
    public event ConditionTrigger OnConditionTrigger;

    protected virtual void CheckCondition() {
        SendActivationEvent();
    }

    protected void SendActivationEvent() {
        conditionMet = true;
        if (active) OnConditionTrigger?.Invoke(data);
        if (singleUse) {
            SetActive(false);
        }
    }

    public void SetActive(bool isActive) {
        active = isActive;
    }

    public bool ConditionIsMet() {
        return conditionMet;
    }
}
