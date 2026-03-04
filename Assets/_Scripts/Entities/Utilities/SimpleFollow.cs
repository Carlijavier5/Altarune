using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    public void Play() {
        enabled = true;
    }

    public void Stop() {
        enabled = false;
    }

    void Update() {
        transform.position = target.position;
    }
}
