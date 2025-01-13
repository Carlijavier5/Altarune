using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleportNexusUIButton : MonoBehaviour
{
    Animator Animator => GetComponent<Animator>();

    [Header("Display")]
    [SerializeField] TMP_Text buttonLabel;

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
    
}
