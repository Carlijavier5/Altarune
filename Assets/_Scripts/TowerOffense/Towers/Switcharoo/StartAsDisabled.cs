using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAsDisabled : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject activeGameObject;
    void Start()
    {
        activeGameObject.SetActive(false);
    }
}
