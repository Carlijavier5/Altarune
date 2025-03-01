using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SovereignCutscene : MonoBehaviour {

    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private NecromancerCharacter necromancer;
    [SerializeField] private Transform teleportTarget;

    void Start() {
        if (GM.ConditionBank.HasSovereignCutscene) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player player)) {
            GM.DialogueManager.DoDialogue(dialogueData);
            GM.DialogueManager.OnDialogueEnd += DialogueManager_OnDialogueEnd;
        }
    }

    private void DialogueManager_OnDialogueEnd() {
        necromancer.TryTeleport(teleportTarget.position);
        GM.ConditionBank.ClearSovereignCutscene();
    }
}