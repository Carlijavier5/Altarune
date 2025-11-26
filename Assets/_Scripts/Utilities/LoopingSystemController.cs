using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingSystemController : MonoBehaviour {

    [SerializeField] private ParticleSystem[] parSystems;

    public void Enable() {
        foreach (ParticleSystem parSystem in parSystems) {
            parSystem.Play();
            ParticleSystem.MainModule main = parSystem.main;
            main.loop = true;
        }
    }

    public void Disable() {
        foreach (ParticleSystem parSystem in parSystems) {
            ParticleSystem.MainModule main = parSystem.main;
            main.loop = false;
        }
    }
}