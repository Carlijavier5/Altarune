using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationOscillator : MonoBehaviour
{
    public float speed = 2f; // Speed of oscillation
    public float angle = 45f; // Maximum angle for oscillation

    private float time;

    void Update()
    {
        time += Time.deltaTime * speed;
        float oscillation = Mathf.Sin(time) * angle;
        transform.localRotation = Quaternion.Euler(0, oscillation, 0);
    }
}
