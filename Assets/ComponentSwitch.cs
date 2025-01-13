using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentSwitch : MonoBehaviour {
    [SerializeField] private Transform component;
    [SerializeField] private CCondition condition;
    [SerializeField] private bool checkOnDialogueEnd = true;
    [SerializeField] private bool setActive = true;
    [SerializeField] private bool singleUse = true;
    private bool used;

    private void Start() {
        if (checkOnDialogueEnd) GM.DialogueManager.OnDialogueEnd += EnableComponent;
        else condition.OnConditionTrigger += EnableAction;
    }

    private void EnableAction(CConditionData data) {
        EnableComponent();
    }

    private void EnableComponent() {
        if (used && singleUse) return;
        used = true;
        if (condition.ConditionIsMet()) {
            if (component.GetComponent<SummonController>()) {
                Debug.Log("summon controller: " + setActive);
                if (setActive) {
                    GM.Player.InputSource.ActivateSummons();
                }
                else {
                    GM.Player.InputSource.DeactivateSummons();
                }
            }
            else {
                Debug.Log("deleted");
                if (condition.ConditionIsMet()) component.gameObject.SetActive(setActive);
            }
        }
    }
}
