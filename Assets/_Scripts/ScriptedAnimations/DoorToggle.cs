using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorToggle : MonoBehaviour {

    [SerializeField] private RoomTag exitTag;
    [SerializeField] private Animator animator;

    public RoomTag ExitTag => exitTag;

    public void Toggle(bool open) {
        if (open) {
            animator.SetBool("OpenDoor", true);
            animator.SetBool("CloseDoor", false);
        } else {
            animator.SetBool("CloseDoor", true);
            animator.SetBool("OpenDoor", false);
        }
    }

    public void ToggleImmediate(bool open) {
        if (open) {
            animator.Play("Base Layer.OpenDoor", 0, 1f);
            animator.SetBool("OpenDoor", true);
            animator.SetBool("CloseDoor", false);
        } else {
            animator.Play("Base Layer.CloseDoor", 0, 1f);
            animator.SetBool("OpenDoor", false);
            animator.SetBool("CloseDoor", true);
        }
    }
}

enum DoorState {
    Open,
    Closed
}