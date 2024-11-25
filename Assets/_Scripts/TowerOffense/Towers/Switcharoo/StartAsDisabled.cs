using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAsDisabled : MonoBehaviour {
    // Start is called before the first frame update
    private GameObject activeGameObject;
    void Start() {
        activeGameObject.SetActive(false);
    }
}
