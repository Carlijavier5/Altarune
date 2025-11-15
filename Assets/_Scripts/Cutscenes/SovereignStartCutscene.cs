using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SovereignStartCutscene : MonoBehaviour {

    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private NecromancerCharacter necromancer;
    [SerializeField] private Transform teleportTarget;
    [SerializeField] private SovereignStartTrigger trigger;
    private bool dialogueRan;

    void Start() {
        if (GM.ConditionBank.HasSovereignCutscene) {
            EnableTrigger();
            Destroy(necromancer.gameObject);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (!dialogueRan
                && other.TryGetComponent(out Player player)) {
            dialogueRan = true;
            GM.DialogueManager.OnDialogueEnd += DialogueManager_OnDialogueEnd;
            ToggleInputs(false);
            GM.DialogueManager.DoDialogue(dialogueData);
        }
    }

    private void DialogueManager_OnDialogueEnd() {
        GM.DialogueManager.OnDialogueEnd -= DialogueManager_OnDialogueEnd;
        necromancer.TryRawTeleport(teleportTarget.position);
        ToggleInputs(true);
        EnableTrigger();
        GM.ConditionBank.ClearSovereignCutscene();
        Destroy(gameObject);
    }

    private void EnableTrigger() => trigger.InitTriggers();

    private void ToggleInputs(bool on) {
        if (!GM.Player) return;
        if (on) {
            GM.Player.InputSource.ActivateInput();
            GM.Player.InputSource.ActivateSummons();
        } else {
            GM.Player.InputSource.DeactivateInput();
            GM.Player.InputSource.DeactivateSummons();
        }
    }
}