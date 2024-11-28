using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceRune : MonoBehaviour {

    [SerializeField] private Renderer runeRenderer;
    [SerializeField] private float fadeTime;
    [SerializeField] private ParticleSystem empParticles;

    public void CollapseRune() {
        StartCoroutine(ICollapseRune());
    }

    private IEnumerator ICollapseRune() {
        empParticles.Play();
        MaterialPropertyBlock mpb = new();
        runeRenderer.GetPropertyBlock(mpb);
        Vector4 color = runeRenderer.sharedMaterial.GetVector("_Color");

        float lerpVal, timer = 0;
        while (timer < fadeTime) {
            timer = Mathf.MoveTowards(timer, fadeTime, Time.deltaTime);
            lerpVal = timer / fadeTime;
            color.w = 1 - lerpVal;
            mpb.SetVector("_Color", color);
            runeRenderer.SetPropertyBlock(mpb);
            yield return null;
        }
    }
}