using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Summon : BaseObject {

    [System.Serializable]
    protected class SummonProperties {
        public Material fadeMaterial;
        public AnimationCurve growthCurveXZ, growthCurveY;
        public float growSpeed = 3;
    } [SerializeField] protected SummonProperties summonProperties = new();

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
            lerpVal = Mathf.MoveTowards(lerpVal, 1, Time.deltaTime * summonProperties.growSpeed);
            t.localScale = new Vector3(summonProperties.growthCurveXZ.Evaluate(lerpVal),
                                       summonProperties.growthCurveY.Evaluate(lerpVal),
                                       summonProperties.growthCurveXZ.Evaluate(lerpVal));
            yield return null;
        }
    }

    public void ToggleHologram(bool on) {
        foreach (KeyValuePair<Renderer, Material[]> kvp in matDict) {
            kvp.Key.sharedMaterials = on ? new Material[] { summonProperties.fadeMaterial } : kvp.Value;
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
        CJUtils.AssetUtils.TryRetrieveAsset(out DefaultSummonProperties properties);
        if (properties) {
            summonProperties.fadeMaterial = properties.fadeMaterial;
            summonProperties.growthCurveXZ = properties.growthCurveXZ;
            summonProperties.growthCurveY = properties.growthCurveY;
        }
    }
    #endif
}