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
  
    private float timer = 0;

    protected override void Update(){
        if (!active) return;
        base.Update();
        if (timer < _attackCooldown) {
            timer += Time.deltaTime;
            return;
        }
        if (!windArea.GetRunning()) return;
            Instantiate(sampleWindEffect, transform.position, transform.rotation);
            windArea.PushNearby(_pushStrength, _pushDuration);
            timer = 0;
    }

 }
    





