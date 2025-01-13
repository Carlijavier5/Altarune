using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UISwitch : MonoBehaviour {
    [Header("Condition Prompts")]
    [SerializeField] private CCondition promptCondition;
    [SerializeField] private bool promptWaitForEnd = true;
    [SerializeField] private CCondition exitCondition;
    [SerializeField] private bool exitWaitForEnd = false;

    [Header("UI Params")] 
    [SerializeField] private float entranceDelay;
    [SerializeField] private float offset = -500f;

    private Vector3 finalLocation;
    private Vector3 exitLocation;
    
    private bool used = false;

    private void Awake() {
        finalLocation = transform.position;
        transform.position = new Vector3(finalLocation.x, finalLocation.y + offset, finalLocation.z);
        exitLocation = transform.position;
    }

    private void Start() {
        if (promptWaitForEnd) GM.DialogueManager.OnDialogueEnd += PromptAction;
        else promptCondition.OnConditionTrigger += ConditionedPromptAction;
        
        if (exitWaitForEnd) GM.DialogueManager.OnDialogueEnd += ExitAction;
        else exitCondition.OnConditionTrigger += ConditionedExitAction;
    }

    private void ConditionedPromptAction(CConditionData data) {
        PromptAction();
    }

    private void PromptAction() {
        if (!used) PromptUI();
    }

    private void ConditionedExitAction(CConditionData data) {
        ExitAction();
    }

    private void ExitAction() {
        if (!used) ExitUI();
    }
    
    

    private void PromptUI() {
        if (promptCondition.ConditionIsMet()) {
            transform.DOMove(finalLocation, entranceDelay);
        }
    }

    private void ExitUI() {
        if (exitCondition.ConditionIsMet()) {
            transform.DOMove(exitLocation, entranceDelay);
            used = true;
        }
    }
    
}
