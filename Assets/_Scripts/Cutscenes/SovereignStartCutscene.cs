using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SovereignStartCutscene : MonoBehaviour {

    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private NecromancerCharacter necromancer;
    [SerializeField] private Transform teleportTarget;
    [SerializeField] private SovereignStartTrigger trigger;

    void Start() {
        if (GM.ConditionBank.HasSovereignCutscene) {
            EnableTrigger();
            Destroy(necromancer.gameObject);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player player)) {
            GM.DialogueManager.DoDialogue(dialogueData);
            ToggleInputs(false);
            GM.DialogueManager.OnDialogueEnd += DialogueManager_OnDialogueEnd;
        }
    }

    private void DialogueManager_OnDialogueEnd() {
        necromancer.TryRawTeleport(teleportTarget.position);
        EnableTrigger();
        GM.ConditionBank.ClearSovereignCutscene();
        ToggleInputs(true);
        Destroy(gameObject);
    }

    private void EnableTrigger() => trigger.InitTriggers();

    private void ToggleInputs(bool on) {
        if (on) {
            GM.Player.InputSource.ActivateInput();
            GM.Player.InputSource.ActivateSummons();
        } else {
            GM.Player.InputSource.DeactivateInput();
            GM.Player.InputSource.DeactivateSummons();
        }
    }
}