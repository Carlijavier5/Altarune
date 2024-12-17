using System;
using TMPro;
using UnityEngine;

public class TextFadeIn : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textField;
    private Entity playerObject;
    Boolean activateText;
    void Update() {
        updateText();
    }

    private void updateText(){
        if(activateText) {
            Debug.Log("I'm trying to change the color " + this.textField.text + " " + textField.alpha);
            textField.alpha = (float) Math.Log(8f / Vector3.Magnitude(playerObject.transform.position - transform.position));

        }
    }
    void OnTriggerEnter(Collider other) {
        activateText = true; 
        if(other.TryGetComponent(out Entity entity)){
            playerObject = entity;
        }
    }

    void OnTriggerExit(Collider other) {
        activateText = false; 
        textField.alpha = 0; 
    }
}
