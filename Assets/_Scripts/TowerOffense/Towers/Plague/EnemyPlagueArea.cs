using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlagueArea : MonoBehaviour {

    [SerializeField] private PlagueStatusEffect plagueEffect;
    [SerializeField] private float growSpeed;
    [SerializeField] private float timeScale;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float range;
    private Vector3 targetScale;
    private Renderer rend;
    public int ParentID { get; set; }

    void Awake() {
        rend = GetComponent<Renderer>();
        targetScale = Vector3.Scale(transform.localScale, new Vector3(range, 1.0f, range));
        transform.localScale = Vector3.zero;
        StartCoroutine(ExpandArea());
    }

    void OnEnable() {
        StartCoroutine(ExpandArea());
    }
 
    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity) && entity.Faction == EntityFaction.Hostile && entity.GetInstanceID() != ParentID) {
                InfectEnemy(other);
        }
    }

    private void InfectEnemy(Collider enemy) {
        Entity entity = enemy.GetComponent<Entity>();
        entity.ApplyEffects(new[] {plagueEffect.Clone() });
    }

    private IEnumerator ExpandArea() {
        while (transform.localScale != targetScale) {
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, growSpeed * Time.deltaTime);
            if (transform.localScale == targetScale) {
                StartCoroutine(FadeOut(0f, fadeDuration));
            }
            yield return null;
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
        this.gameObject.SetActive(false);
    }
}