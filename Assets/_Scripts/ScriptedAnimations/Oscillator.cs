using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour {

    [SerializeField] private float amplitude, speed;
    [SerializeField] float timeOffset;
    private float timeScale = 1;
    private Vector3 anchor;

    void Awake() {
        anchor = transform.localPosition;
    }

    void Update() {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition,
                                                      anchor + Mathf.Sin(Time.time * speed + timeOffset) * amplitude * Vector3.up,
                                                      Time.deltaTime * speed * timeScale);
    }

    public void SetTimeScale(float timeScale) => this.timeScale = timeScale;
}
