using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallback : MonoBehaviour 
{
    private bool isFirstUpdate = true;

    void Update() {
        if (isFirstUpdate) {
            isFirstUpdate = false;
            SceneLoader.Instance.LoaderCallback();
        }
    }
}
