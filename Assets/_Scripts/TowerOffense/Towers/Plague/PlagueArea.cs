using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlagueArea : MonoBehaviour {

    [SerializeField] private PlagueStatusEffect plagueEffect;
    [SerializeField] private ParticleSystem parSystem;
    [SerializeField] private SphereCollider contactCollider;
    [SerializeField] private float growDuration;
    [SerializeField] private Vector2 startEndRange;

    public float EntityID { get; set; }

    public void DoWave() {
        parSystem.Play();
        contactCollider.enabled = true;
        StartCoroutine(IExpandWave());
    }

    void OnTriggerEnter(Collider other) {
        if (other.GetInstanceID() != EntityID
            && other.TryGetComponent(out Entity entity)
                && entity.IsFaction(EntityFaction.Hostile)) {
            InfectEnemy(entity);
        }
    }

    private void InfectEnemy(Entity entity) {
        entity.ApplyEffects(new[] { plagueEffect.Clone() as EntityEffect });
    }

    private IEnumerator IExpandWave() {
        float lerpVal, timer = 0;
        while (timer < growDuration) {
            timer += Time.deltaTime;
            lerpVal = timer / growDuration;
            contactCollider.radius = Mathf.Lerp(startEndRange.x,
                                                startEndRange.y, lerpVal);
            yield return null;
        }

        contactCollider.enabled = false;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, startEndRange.x);
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, startEndRange.y);
        UnityEditor.Handles.color = Color.white;
    }
    #endif
}

[System.Serializable]
public class PlagueStatusEffect : EntityEffect {

    [SerializeField] private HealthAttributeModifiers attMods;
    [SerializeField] private PlagueArea plagueAreaPrefab;
    [SerializeField] private Material material;
    [SerializeField] private float maxDuration;
    [SerializeField] private int plagueDamage;
    [SerializeField] private float damageInterval, spreadInterval;

    private float durationTimer, damageTimer,
                  spreadTimer;
    private PlagueArea plagueArea;

    public override void Apply(Entity entity, bool isNew) {
        if (isNew) {
            entity.ApplyMaterial(material);
            HealthModifiers = attMods;
        } else {
            durationTimer = 0;
        }
    }

    public override bool Update(Entity entity) {
        durationTimer += Time.deltaTime;
        damageTimer += Time.deltaTime;
        spreadTimer += Time.deltaTime;
        if (damageTimer >= damageInterval) {
            entity.TryDamage(plagueDamage);
            damageTimer = 0;
        }

        if (spreadTimer >= spreadInterval) {
            if (plagueArea == null) {
                plagueArea = Object.Instantiate(plagueAreaPrefab, entity.transform.position,
                                                Quaternion.identity, entity.transform);
                plagueArea.EntityID = entity.GetInstanceID();
            }
            plagueArea.DoWave();
            spreadTimer = 0;
        }
        return durationTimer >= maxDuration;
    }

    public override void Terminate(Entity entity) {
        entity.RemoveMaterial(material);
    }
}