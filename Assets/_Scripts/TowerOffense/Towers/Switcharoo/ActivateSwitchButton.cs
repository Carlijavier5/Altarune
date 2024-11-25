using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateSwitchButton : MonoBehaviour {
    private GameObject activeGameObject;

    private void OnMouseUpAsButton() {
        if (activeGameObject.activeSelf != true) {
            activeGameObject.SetActive(true);
        }
        else {
            activeGameObject.SetActive(false);
        }
    }
}
