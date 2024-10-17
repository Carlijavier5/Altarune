using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlagueArea : MonoBehaviour {

    [SerializeField] private PlagueStatusEffect plagueEffect;
    [SerializeField] private float growSpeed;
    [SerializeField] private float timeScale;
    [SerializeField] private float plagueInterval;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float range;
    private Vector3 targetScale;
    private Renderer rend;

    void Awake() {
        rend = GetComponent<Renderer>();
        targetScale = Vector3.Scale(transform.localScale, new Vector3(range, 1.0f, range));
        transform.localScale = Vector3.zero;
        StartCoroutine(ExpandArea());
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity) && entity.Faction == EntityFaction.Hostile) {
            InfectEnemy(other);
        }
    }

    private void InfectEnemy(Collider enemy) {
        Entity entity = enemy.GetComponent<Entity>();
        entity.ApplyEffects(new[] {plagueEffect.Clone() });
    }

    private IEnumerator ExpandArea() {
        while (true) {
            while (transform.localScale != targetScale) {
                transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, growSpeed * Time.deltaTime);
                if (transform.localScale == targetScale) {
                    StartCoroutine(FadeOut(0f, fadeDuration));
                }
                yield return null;
            }
            yield return new WaitForSeconds(plagueInterval);
        }
    }

    private IEnumerator FadeOut(float targetAlpha, float duration) {
        float initialAlpha = rend.material.GetFloat("_Alpha");
        float timeElapsed = 0f;

        while (timeElapsed < duration) {
            timeElapsed += Time.deltaTime;
            rend.material.SetFloat("_Alpha", Mathf.Lerp(initialAlpha, targetAlpha, timeElapsed / duration));
            yield return null;
        }
        transform.localScale = Vector3.zero;
        rend.material.SetFloat("_Alpha", 0.7f);
    }
}

[System.Serializable]
public class PlagueStatusEffect : StatusEffect {

    [SerializeField] private HealthAttributeModifiers attMods;
    [SerializeField] private EnemyPlagueArea enemyPlagueArea;
    [SerializeField] private Material material;
    [SerializeField] private float maxDuration;
    [SerializeField] private int plagueDamage;
    [SerializeField] private float damageInterval;
    [SerializeField] private float spreadInterval;
    private float durationTimer;
    private float damageTimer;
    private float spreadTimer;
    private EnemyPlagueArea plagueArea;

    public override void Apply(Entity entity, bool isNew) {
        if (isNew) {
            entity.SetMaterial(material);
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
            damageTimer = 0f;
        }
        if (spreadTimer >= spreadInterval) {
            EnemyPlagueArea plagueArea = entity.GetComponentInChildren<EnemyPlagueArea>(true);
            if (plagueArea == null) {
                plagueArea = Object.Instantiate(enemyPlagueArea, entity.transform.position, Quaternion.identity);
                plagueArea.transform.parent = entity.transform;
                plagueArea.ParentID = entity.GetInstanceID();
            } else {
                plagueArea.gameObject.SetActive(true);
            }
            spreadTimer = 0f;
        }
        return durationTimer >= maxDuration;
    }

    public override void Terminate(Entity entity) {
        entity.ResetMaterials();
    }
}