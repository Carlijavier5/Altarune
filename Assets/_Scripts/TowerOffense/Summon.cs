using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Summon : BaseObject {

    [SerializeField] private DefaultSummonProperties settings;
    [SerializeField] protected float manaDepletion = 1f;

    public Entity Summoner { get; protected set; }

    protected ManaSource manaSource;
    protected bool active;

    public virtual void Init(Entity summoner,
                             ManaSource manaSource) {
        this.manaSource = manaSource;
        Summoner = summoner;
        active = true;
    }

    public virtual void Collapse() {
        active = false;
        StartCoroutine(AnimateObjectDespawn());
    }

    protected virtual void Update() {
        if (active) manaSource.Drain(Time.deltaTime * manaDepletion);
    }

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

    private IEnumerator AnimateObjectDespawn() {
        Transform t = transform;
        t.localScale = Vector3.zero;

        float lerpVal = 1;
        while (lerpVal > 0) {
            lerpVal = Mathf.MoveTowards(lerpVal, 0, Time.deltaTime * settings.growSpeed);
            t.localScale = new Vector3(settings.growthCurveXZ.Evaluate(lerpVal),
                                       settings.growthCurveY.Evaluate(lerpVal),
                                       settings.growthCurveXZ.Evaluate(lerpVal));
            yield return null;
        }
        Destroy(gameObject, 0.2f);
    }

    #if UNITY_EDITOR
    void Reset() {
        CJUtils.AssetUtils.TryRetrieveAsset(out DefaultSummonProperties settings);
        this.settings = settings;
    }
    #endif
}