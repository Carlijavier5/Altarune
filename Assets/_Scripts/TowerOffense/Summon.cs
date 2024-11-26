using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public abstract partial class Summon : BaseObject {

    //private const string HOLO_THRESHOLD_KEY = "_Teleport_Threshold";

    [SerializeField] private DefaultSummonProperties settings;
    [SerializeField] protected float manaDepletion = 1f;

    //private VisualEffect hologramVFX;

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
        StopAllCoroutines();
        StartCoroutine(ISummonMaterialize(false));
    }

    protected virtual void Update() {
        if (active) manaSource.Drain(Time.deltaTime * manaDepletion);
    }

    public void DoSpawn() {
        objectBody.localScale = Vector3.zero;
        StartCoroutine(ISummonMaterialize(true));
    }

    private IEnumerator ISummonMaterialize(bool spawn) {
        float target = spawn ? settings.growTime : 0;
        Transform t = objectBody;

        /*
        if (!hologramVFX) {
            hologramVFX = Instantiate(settings.hologramVFX, transform);
        } hologramVFX.Play();
        ApplyMaterial(settings.hologramMaterial);
        */

        float lerpVal, growTimer = settings.growTime - target;
        while (Mathf.Abs(growTimer - target) > Mathf.Epsilon) {
            growTimer = Mathf.MoveTowards(growTimer, target, Time.deltaTime);
            lerpVal = growTimer / settings.growTime;
            t.localScale = new Vector3(settings.growthCurveXZ.Evaluate(lerpVal),
                                       settings.growthCurveY.Evaluate(lerpVal),
                                       settings.growthCurveXZ.Evaluate(lerpVal));
            //UpdatePropertyBlock((mpb) => { mpb.SetFloat(HOLO_THRESHOLD_KEY, 1 - lerpVal); });
            yield return null;
        }

        if (spawn) { }/*RemoveMaterial(settings.hologramMaterial);*/
        else Destroy(gameObject, 0.2f);
    }

    #if UNITY_EDITOR
    void Reset() {
        CJUtils.AssetUtils.TryRetrieveAsset(out DefaultSummonProperties settings);
        this.settings = settings;
    }
    #endif
}