using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTemporal : Summon {

    [SerializeField] private TemporalArea tempoArea;
    [SerializeField] private TimeMagicCircleController magicCircleController;
    [SerializeField] private HourglassController hourglassController;
    [Range(0, 1)] [SerializeField] private float slowMultiplier;
    [SerializeField] private float slowDuration, effectRadius, tempoSpawnDelay;

    private readonly Dictionary<Collider, BaseObject> objectMap = new();

    void Awake() {
        magicCircleController.SetRadius(0);
    }

    public override void Init(SummonData data, Entity summoner,
                              ManaSource manaSource) {
        base.Init(data, summoner, manaSource);
        StartCoroutine(IDevelopTempoArea());
    }

    void Update() {
        if (active) {
            float lerp = 0.2f + Mathf.Abs(Mathf.PingPong(Time.time, 0.8f));
            hourglassController.SetFill(lerp);
        }
    }

    private IEnumerator IDevelopTempoArea() {
        yield return new WaitForSeconds(tempoSpawnDelay);
        InitArea();

        float radius, lerpVal = 0;
        while (lerpVal < 1) {
            lerpVal = Mathf.MoveTowards(lerpVal, 1, Time.deltaTime);
            radius = Mathf.Lerp(0, effectRadius, lerpVal);
            magicCircleController.SetRadius(radius);
            tempoArea.SetRadius(radius);
            yield return null;
        }
    }

    private void TemporalArea_OnContactStay(Collider other) {
        if (objectMap.ContainsKey(other)) {
            ApplyEffect(objectMap[other]);
        } else if (other.TryGetComponent(out BaseObject baseObject)) {
            objectMap[other] = baseObject;
            ApplyEffect(baseObject);
        }
    }

    private void TemporalArea_OnContactExit(Collider other) => objectMap.Remove(other);

    private void ApplyEffect(BaseObject baseObject) {
        TemporalDistortionEffect effect = new(slowMultiplier, slowDuration);
        if (baseObject is Entity) {
            Entity entity = baseObject as Entity;
            entity.ApplyEffects(new[] { effect });
        } else if (baseObject is Summon) {
            // Apply effect to towers;
        }
    }

    private void InitArea() {
        tempoArea.gameObject.SetActive(true);
        tempoArea.OnContactStay += TemporalArea_OnContactStay;
        tempoArea.OnContactExit += TemporalArea_OnContactExit;
    }
}