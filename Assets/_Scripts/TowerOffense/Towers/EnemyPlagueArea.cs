using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlagueArea : MonoBehaviour {

    [SerializeField] private PlagueStatusEffect plagueEffect;
    [SerializeField] private float growSpeed;
    [SerializeField] private float timeScale;
    [SerializeField] private float plagueInterval;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float range;
    private Vector3 targetScale;
    private Renderer rend;
    public int parentID;
    private List<Collider> enemiesInArea = new List<Collider>();

    void Awake() {
        rend = GetComponent<Renderer>();
        targetScale = Vector3.Scale(transform.localScale, new Vector3(range, 1.0f, range));
        transform.localScale = Vector3.zero;
    }

    void OnEnable() {
        rend.material.SetFloat("_Alpha", 0.7f);
        transform.localScale = Vector3.zero;
    }

    void Update() {
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, growSpeed * Time.deltaTime);
        if(transform.localScale == targetScale) {
            StartCoroutine(InfectEnemies());
            StartCoroutine(FadeOut(0f, fadeDuration));
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity) && entity.Faction == EntityFaction.Hostile && entity.GetInstanceID() != parentID) {
            if(!enemiesInArea.Contains(other)) {
                enemiesInArea.Add(other);
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (enemiesInArea.Contains(other)) {
            enemiesInArea.Remove(other);
        }
    }

    private IEnumerator InfectEnemies() {
            foreach (Collider enemy in enemiesInArea) {
                if (enemy != null) {
                    Entity entity = enemy.GetComponent<Entity>();
                    entity.ApplyEffects(new[] {plagueEffect.Clone() });
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
        rend.material.SetFloat("_Alpha", targetAlpha);
        enemiesInArea.Clear();
        this.gameObject.SetActive(false);
    }
}