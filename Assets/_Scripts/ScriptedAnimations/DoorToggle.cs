using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorToggle : MonoBehaviour
{
    [SerializeField]
    DoorState initialState;
    DoorState currentState;
    public Animator doorAnimator;

    public UnityEvent DoorToggled;
    // Start is called before the first frame update
    void Start()
    {
        if (DoorToggled == null)
            DoorToggled = new UnityEvent();
        
        if (initialState == DoorState.Open) {
            doorAnimator.Play("Base Layer.OpenDoor", 0, 1f);
            doorAnimator.SetBool("OpenDoor", true);
            doorAnimator.SetBool("CloseDoor", false);
        } else {
            doorAnimator.Play("Base Layer.CloseDoor", 0, 1f);
            doorAnimator.SetBool("OpenDoor", false);
            doorAnimator.SetBool("CloseDoor", true);
        }
        currentState = initialState;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleDoorState() {
        DoorToggled.Invoke();
        if (currentState == DoorState.Closed) {
            currentState = DoorState.Open;
            doorAnimator.SetBool("OpenDoor", true);
            doorAnimator.SetBool("CloseDoor", false);
        } else {
            currentState = DoorState.Closed;
            doorAnimator.SetBool("CloseDoor", true);
            doorAnimator.SetBool("OpenDoor", false);
        }
    }
}

enum DoorState {
    Open,
    Closed
}