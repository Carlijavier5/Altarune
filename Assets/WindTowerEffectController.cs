using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WindTowerEffectController : MonoBehaviour {
    [SerializeField] private RotateObject rotator;

    [SerializeField] private ParticleSystem windIn;

    [SerializeField] private ParticleSystem windOut;

    [SerializeField] private ParticleSystem windVortex;
    private bool _activeVortex;
    public bool debugMode;
    private float initSpeed;

    private void Awake() {
        windIn.Stop();
        windOut.Stop();
        windVortex.Stop();
        initSpeed = rotator.speed;
    }

    private void Update() {
        if (debugMode) {
            if (Input.GetKeyDown(KeyCode.I)) StartCoroutine(RunInVortex());
            else if (Input.GetKeyDown(KeyCode.O)) StartCoroutine(RunOutVortex());
            else if (Input.GetKeyDown(KeyCode.P)) StartCoroutine(RunActiveVortex());
            else if (Input.GetKeyDown(KeyCode.L)) Deactivate();
        }
    }


    public enum WindMode {
        In,
        Out,
        Vortex
    };

    /// <summary>
    /// Spawn designated effect based on attack mode.
    /// </summary>
    /// <param name="mode">In, Out, Vortex</param>
    public void Activate(WindMode mode) {
        if (mode == WindMode.In) StartCoroutine(RunInVortex());
        else if (mode == WindMode.Out) StartCoroutine(RunOutVortex());
        else if (mode == WindMode.Vortex) StartCoroutine(RunActiveVortex());
    }

    /// <summary>
    /// Disables all active FX
    /// </summary>
    public void Deactivate() {
        windIn.Stop();
        windOut.Stop();
        windVortex.Stop();
        if (_activeVortex) {
            rotator.speed = initSpeed;
        }
    }

    private IEnumerator RunInVortex() {
        windIn.Clear();
        windIn.Play();
        rotator.speed = initSpeed * 10f;
        yield return new WaitForSeconds(1f);
        rotator.speed = initSpeed;
    }
    
    private IEnumerator RunOutVortex() {
        windOut.Clear();
        windOut.Play();
        rotator.speed = initSpeed * -10f;
        yield return new WaitForSeconds(1f);
        rotator.speed = initSpeed;
    }

    private IEnumerator RunActiveVortex() {
        if (!_activeVortex) {
            _activeVortex = true;
            windVortex.Clear();
            windVortex.Play();
            rotator.speed = initSpeed * 20f;
        }
        yield return null;
    }
}
