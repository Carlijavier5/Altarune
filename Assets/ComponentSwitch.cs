using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentSwitch : MonoBehaviour {
    [SerializeField] private Transform component;
    [SerializeField] private CCondition condition;
    [SerializeField] private bool checkOnDialogueEnd = true;
    [SerializeField] private bool setActive = true;

    private void Start() {
        if (checkOnDialogueEnd) GM.DialogueManager.OnDialogueEnd += EnableComponent;
        else condition.OnConditionTrigger += EnableAction;
    }

    private void EnableAction(CConditionData data) {
        EnableComponent();
    }

    private void EnableComponent() {
        if (condition.ConditionIsMet()) component.gameObject.SetActive(setActive);
    }
}
