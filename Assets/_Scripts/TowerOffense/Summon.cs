using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Summon : BaseObject {

    [SerializeField] private DefaultSummonProperties settings;
    private readonly Dictionary<Renderer, Material[]> matDict = new();

    protected virtual void Awake() {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers) {
            matDict[renderer] = renderer.sharedMaterials;
        }
    }

    public abstract void Init();

    public void DoSpawnAnim() => StartCoroutine(AnimateObjectSpawn());

    private IEnumerator AnimateObjectSpawn() {
        Transform t = transform;
        t.localScale = Vector3.zero;

        float lerpVal = 0;
        while (lerpVal < 1) {
            lerpVal = Mathf.MoveTowards(lerpVal, 1, Time.deltaTime * settings.growSpeed);
            t.localScale = new Vector3(settings.growthCurveXZ.Evaluate(lerpVal),
                                       settings.growthCurveY.Evaluate(lerpVal),
                                       settings.growthCurveXZ.Evaluate(lerpVal));
            yield return null;
        }
    }

    public void ToggleHologram(bool on) {
        foreach (KeyValuePair<Renderer, Material[]> kvp in matDict) {
            kvp.Key.sharedMaterials = on ? new Material[] { settings.fadeMaterial } : kvp.Value;
        }
    }

    public void ToggleHologramRed(bool doRed) {
        foreach (KeyValuePair<Renderer, Material[]> kvp in matDict) {
            MaterialPropertyBlock mpb = new();
            if (doRed) mpb.SetColor("_BaseColor", Color.red);
            kvp.Key.SetPropertyBlock(mpb);
        }
    }

    #if UNITY_EDITOR
    void Reset() {
        CJUtils.AssetUtils.TryRetrieveAsset(out DefaultSummonProperties settings);
        this.settings = settings;
    }
    #endif
}