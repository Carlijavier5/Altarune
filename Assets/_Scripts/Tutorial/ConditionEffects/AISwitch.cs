using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AISwitch : MonoBehaviour
{
    private enum Actions {
        Roam,
        Freeze,
        Continue
    }

    [SerializeField] private CCondition condition;
    [SerializeField] private Actions action;
    [SerializeField] private Entity entity;
    [SerializeField] private Transform aggroRange;
    [SerializeField] private bool checkOnDialogueEnd = true;

    private bool eventBypassed = false;
    private void Start() {
        if (checkOnDialogueEnd) GM.DialogueManager.OnDialogueEnd += SetAction;
        else condition.OnConditionTrigger += SetConditionAction;
    }

    void OnDestroy() {
        GM.DialogueManager.OnDialogueEnd -= SetAction;
        condition.OnConditionTrigger -= SetConditionAction;
    }

    private void SetConditionAction(CConditionData data) {
        eventBypassed = true;
        SetAction();
    }

    private void SetAction() {
        if (condition.ConditionIsMet() || eventBypassed) {
            switch (action) {
                case Actions.Roam:
                    entity.enabled = true;
                    if (aggroRange) aggroRange.gameObject.SetActive(false);
                    break;
                case Actions.Freeze:
                    entity.enabled = false;
                    break;
                case Actions.Continue:
                    if (!entity) break; 
                    entity.enabled = true;
                    if (aggroRange) aggroRange.gameObject.SetActive(true);
                    break;
            }
        }
    }
}
