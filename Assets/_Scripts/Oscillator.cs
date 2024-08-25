using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour {

    [SerializeField] private float amplitude, speed;
    private Vector3 anchor;

    void Awake() {
        anchor = transform.localPosition;
    }

    void Update() {
        transform.localPosition = anchor + Mathf.Sin(Time.time * speed) * amplitude * Vector3.up;
    }
}
