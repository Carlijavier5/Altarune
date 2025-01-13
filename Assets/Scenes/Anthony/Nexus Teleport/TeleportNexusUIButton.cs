using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleportNexusUIButton : MonoBehaviour
{
    Animator Animator => GetComponent<Animator>();

    [Header("Display")]
    [SerializeField] TMP_Text buttonLabel;
    [SerializeField] RoomTag roomTag;
    public RoomTag RoomTag => roomTag;
    
    public void SetButtonLabel(string label) {
        buttonLabel.text = label;
    }
    
    public void ShowButton()
    {
        if (Animator) {
            Animator.SetTrigger("show");
        }
    }

    public void HideButton() {
        if (Animator) {
            Animator.SetTrigger("hide");
        }
    }

    public void TeleportToRoom() {
        GM.RoomManager.MoveToRoom(roomTag);
    }
}
