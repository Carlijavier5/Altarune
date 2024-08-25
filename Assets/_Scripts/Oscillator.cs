using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour {

    [SerializeField] private float amplitude, speed;
    private Vector3 anchor;

    void Awake() {
        anchor = transform.position;
    }

    void Update() {
        transform.position = anchor + Mathf.Sin(Time.time * speed) * amplitude * Vector3.up;
    }
}
