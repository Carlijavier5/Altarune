using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallGatlingArea : MonoBehaviour {
    private float damageInterval;
    private int damage;
    private GatlingGun gatlingGun;
    private float minSize;
    private float maxSize;
    private float duration;
    private bool init;

    void OnEnable() {
        if (init) {
            float randomSize = Random.Range(minSize, maxSize);
            transform.localScale = new Vector3(randomSize, 0.1f, randomSize);
            Invoke("Expire", duration);
            StartCoroutine(Damage());
        }
    }

    public void Init(float duration, int damage, float damageInterval, float minSize, float maxSize, GatlingGun gatlingGun) {
        init = true;
        this.duration = duration;
        this.minSize = minSize;
        this.maxSize = maxSize;
        this.damageInterval = damageInterval;
        this.damage = damage;
        this.gatlingGun = gatlingGun;
        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, 0.1f, randomSize);
        Invoke("Expire", duration);
        StartCoroutine(Damage());
    }

    private IEnumerator Damage() {
        while (true) {
            yield return new WaitForSeconds(damageInterval);
            DealDamage();
        }
    }

    private void DealDamage() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, transform.localScale.x / 2f);
        foreach (Collider collider in hitColliders) {
            if (collider.TryGetComponent(out Entity entity)) {
                if (entity.Faction != EntityFaction.Friendly) {
                    entity.TryDamage(damage);
                }
            }
        }
    }

    private void Expire() {
        gatlingGun.NumOfInactive++;
        this.gameObject.SetActive(false);
    }
}