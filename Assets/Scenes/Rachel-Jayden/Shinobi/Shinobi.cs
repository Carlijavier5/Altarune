using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shinobi : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.SetPositionAndRotation(transform.position * 2, transform.rotation);
    }
}
