using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CConditionSwitch : MonoBehaviour {
    [SerializeField] private CCondition sender;
    [SerializeField] private CCondition receiver;
    [SerializeField] private bool setActive = true;

    private bool used = false;
    private void Start() {
        GM.DialogueManager.OnDialogueEnd += ActivateCondition;
    }

    private void ActivateCondition() {
        if (sender.ConditionIsMet() && !used) {
            receiver.SetActive(setActive);
            used = true;
        }
    }
}
