using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTempo : Summon {

    [SerializeField] private TempoArea tempoArea;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float tempoSpawnDelay,
                                   gyroMaxSpeed,
                                   gyroAccel;
    [SerializeField] private Transform gyroOuter, gyroInner;

    private float gyroSpeed;
    private bool init;

    public override void Init() {
        init = true;
        StartCoroutine(ISpawnTempoArea());
    }

    void Update() {
        if (init) {
            gyroSpeed = Mathf.MoveTowards(gyroSpeed, gyroMaxSpeed, Time.deltaTime * gyroAccel);
            gyroOuter.Rotate(gyroSpeed * Time.deltaTime * Vector3.one);
            gyroInner.Rotate(-2 * gyroSpeed * Time.deltaTime * Vector3.one);
        }
    }

    private IEnumerator ISpawnTempoArea() {
        yield return new WaitForSeconds(tempoSpawnDelay);
        Instantiate(tempoArea, launchPoint.transform.position, Quaternion.identity);
    }
}

public class TempoArea : MonoBehaviour {

    [SerializeField] private float growSpeed;
    private Vector3 targetScale;

    void Awake() {
        targetScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    void Update() {
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, growSpeed * Time.deltaTime);
    }
}