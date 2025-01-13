using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class LinkedCondition : CCondition {
    [SerializeField] private List<CCondition> preconditionList;
    
    private void Awake() {
        RegisterConditions();
    }

    private void RegisterConditions() {
        foreach (CCondition condition in preconditionList) {
            condition.OnConditionTrigger += HandleConditions;
        }
    }

    private void HandleConditions(CConditionData data) {
        int conditionsMet = 0;
        foreach (CCondition condition in preconditionList) {
            if (condition.ConditionIsMet()) {
                conditionsMet++;
            }
        }
        
        if (conditionsMet >= preconditionList.Count) CheckCondition();
    }

    protected override void CheckCondition() {
        SendActivationEvent();
    }
}
