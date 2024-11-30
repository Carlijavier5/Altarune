using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class TowerWind : Summon
{
  
    [SerializeField] private SampleWindEffect sampleWindEffect;
    [SerializeField] private WindArea windArea;
    [SerializeField] private float _attackCooldown;

    [SerializeField] private float _pushDuration;
    [SerializeField] private float _pushStrength;

    [SerializeField] private float _pullSpeed;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _pullTimer;
    [SerializeField] private bool _pullMode = false;

    private float timer = 0;
    private bool running = false;
    private float timeRemaining;

    private void Start() {
        timeRemaining = _pullTimer;
    }


    protected override void Update() {
        if (!active) return;
        base.Update();
        windArea.changeRadius(_pullMode);
       //Debug.Log(timer);

        if (!_pullMode) {

            if (timer < _attackCooldown) {
                    timer += Time.deltaTime;
                    return;
            }

            if (!windArea.GetRunning()) return;
            Instantiate(sampleWindEffect, transform.position, transform.rotation);
            windArea.PushNearby(_pushStrength, _pushDuration);


        }
        else {

            if (timer < _attackCooldown * 1.5) {
                if (!running) {
                    timer += Time.deltaTime;
                    return;
                }
            }

            running = true;

            if (timeRemaining > 0) {
                timeRemaining -= Time.deltaTime;
                windArea.PullNearby(_pullSpeed, _rotateSpeed, _pullTimer);
                //Debug.Log("pulling");
                return;
            }
            running = false;
            timeRemaining = _pullTimer;
            //Debug.Log("finished");
        }
        timer = 0;
    }
}
    





