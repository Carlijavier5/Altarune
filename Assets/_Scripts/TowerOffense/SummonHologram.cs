using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonHologram : MonoBehaviour {

    private Renderer[] renderers;

    protected virtual void Awake() {
        renderers = GetComponentsInChildren<Renderer>(true);
    }

    public void ToggleHologramRed(bool doRed) {
        foreach (Renderer renderer in renderers) {
            MaterialPropertyBlock mpb = new();
            if (doRed) mpb.SetColor("_BaseColor", Color.red);
            renderer.SetPropertyBlock(mpb);
        }
    }
}
