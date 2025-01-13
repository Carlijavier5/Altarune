using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class DoorToggle : MonoBehaviour {

    [SerializeField] private RoomTag exitTag;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform doorPanel;
    [SerializeField] private Vector3 movePos;
    [SerializeField] private float time;

    private Vector3 initPos;

    private void Awake() {
        initPos = doorPanel.position;
    }

    public RoomTag ExitTag => exitTag;

    public void Toggle(bool open) {
        if (open) {
            // animator.SetBool("OpenDoor", true);
            // animator.SetBool("CloseDoor", false);
            OpenAnim();
        } else {
            // animator.SetBool("CloseDoor", true);
            // animator.SetBool("OpenDoor", false);
            CloseAnim();
        }
    }

    public void ToggleImmediate(bool open) {
        if (open) {
            // animator.Play("Base Layer.OpenDoor", 0, 1f);
            // animator.SetBool("OpenDoor", true);
            // animator.SetBool("CloseDoor", false);
            OpenAnim();
        } else {
            // animator.Play("Base Layer.CloseDoor", 0, 1f);
            // animator.SetBool("OpenDoor", false);
            // animator.SetBool("CloseDoor", true);
            CloseAnim();
        }
    }

    private void OpenAnim() {
        doorPanel.DOMove(doorPanel.position + movePos, time);
    }
    
    private void CloseAnim() {
        if (initPos != Vector3.zero) doorPanel.DOMove(initPos, time);
    }
}

enum DoorState {
    Open,
    Closed
}