using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTemporal : Summon {

    [SerializeField] private TempoArea tempoArea;
    [SerializeField] private TimeMagicCircleController magicCircleController;
    [SerializeField] private HourglassController hourglassController;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float tempoSpawnDelay;

    private bool init;

    void Start() {
        magicCircleController.SetRadiusLerp(0);
    }

    public override void Init() {
        init = true;
        StartCoroutine(ISpawnTempoArea());
    }

    void Update() {
        if (init) {
            float lerp = 0.2f + Mathf.Abs(Mathf.PingPong(Time.time, 0.8f));
            hourglassController.SetFill(lerp);
        }
    }

    private IEnumerator ISpawnTempoArea() {
        yield return new WaitForSeconds(tempoSpawnDelay);
        Instantiate(tempoArea, launchPoint.transform.position, Quaternion.identity);
        float lerpVal = 0;
        while (lerpVal < 1) {
            lerpVal = Mathf.MoveTowards(lerpVal, 1, Time.deltaTime);
            magicCircleController.SetRadiusLerp(lerpVal);
            yield return null;
        }
    }
}