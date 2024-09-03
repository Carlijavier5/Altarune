using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour {

    [SerializeField] private float amplitude, speed;
    private float timeScale = 1;
    private Vector3 anchor;

    void Awake() {
        anchor = transform.localPosition;
    }

    void Update() {
        transform.localPosition = anchor + Mathf.Sin(Time.time * timeScale * speed) * amplitude * Vector3.up;
    }

    public void SetTimeScale(float timeScale) => this.timeScale = timeScale;
}
