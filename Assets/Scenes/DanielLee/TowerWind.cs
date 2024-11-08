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
  
    private bool init = false;
    private float timer = 0;

    public override void Init(Player player) {
        base.Init(player);
        init = true;
    }

    protected override void Update(){
        if (!init) return;
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
    





