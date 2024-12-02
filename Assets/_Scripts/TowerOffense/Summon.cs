using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public abstract partial class Summon : BaseObject {

    public event System.Action<Summon> OnSummonCollapse;

    //private const string HOLO_THRESHOLD_KEY = "_Teleport_Threshold";
    //private VisualEffect hologramVFX;

    [SerializeField] private DefaultSummonProperties settings;
    private SummonData data;

    public Entity Summoner { get; protected set; }

    protected ManaSource manaSource;
    protected bool active;

    public virtual void Init(SummonData data, Entity summoner,
                             ManaSource manaSource) {
        this.manaSource = manaSource;
        this.data = data;

        Summoner = summoner;
        manaSource.OnManaTax += ManaSource_OnManaTax;
        active = true;
    }

    private void ManaSource_OnManaTax(EventResponse<float> eRes) {
        if (active) eRes.objectReference += data.manaDrain;
    }

    public virtual void Collapse() {
        active = false;
        OnSummonCollapse?.Invoke(this);
        manaSource.OnManaTax -= ManaSource_OnManaTax;

        StopAllCoroutines();
        StartCoroutine(ISummonMaterialize(false));
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