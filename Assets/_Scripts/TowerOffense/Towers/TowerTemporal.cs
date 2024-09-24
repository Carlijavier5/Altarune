using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTemporal : Summon {

    [SerializeField] private TempoArea tempoArea;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float tempoSpawnDelay,
                                   gyroMaxSpeed,
                                   gyroAccel;
    [SerializeField] private Transform gyroOuter, gyroInner;

    private Oscillator oscillator;
    private float gyroSpeed;
    private bool init;

    protected override void Awake() {
        base.Awake();
        oscillator = GetComponentInChildren<Oscillator>(true);
        oscillator.enabled = false;
    }

    public override void Init() {
        init = true;
        oscillator.enabled = true;
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