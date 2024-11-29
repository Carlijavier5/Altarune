using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTest : MonoBehaviour {

    [SerializeField] private DialogueData data;

    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            GM.DialogueManager.DoDialogue(data);
        }
    }
}