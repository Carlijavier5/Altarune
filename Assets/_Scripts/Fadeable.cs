using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fadeable : MonoBehaviour {
    [SerializeField] private Material fadeMat;
    [SerializeField] private AnimationCurve growthCurveXZ, growthCurveY;
    [SerializeField] private float growSpeed;
    private Dictionary<Renderer, Material[]> matDict = new();

    protected virtual void Awake() {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers) {
            matDict[renderer] = renderer.sharedMaterials;
        }
    }

    public void DoSpawnAnim() {
        StartCoroutine(AnimateObjectSpawn());
    }

    private IEnumerator AnimateObjectSpawn() {
        Transform t = transform;
        t.localScale = Vector3.zero;

        float lerpVal = 0;
        while (lerpVal < 1) {
            lerpVal = Mathf.MoveTowards(lerpVal, 1, Time.deltaTime * growSpeed);
            t.localScale = new Vector3(growthCurveXZ.Evaluate(lerpVal),
                                       growthCurveY.Evaluate(lerpVal),
                                       growthCurveXZ.Evaluate(lerpVal));
            yield return null;
        }
    }

    protected void SwapFade(bool on) {
        foreach (KeyValuePair<Renderer, Material[]> kvp in matDict) {
            kvp.Key.sharedMaterials = on ? new Material[] { fadeMat } : kvp.Value;
        }
    }

    public void PaintRed(bool doRed) {
        foreach (KeyValuePair<Renderer, Material[]> kvp in matDict) {
            MaterialPropertyBlock mpb = new();
            if (doRed) mpb.SetColor("_BaseColor", Color.red);
            kvp.Key.SetPropertyBlock(mpb);
        }
    }
}
