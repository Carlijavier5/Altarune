using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallGatlingArea : MonoBehaviour {
    private float damageInterval;
    private int damage;
    private GatlingGun gatlingGun;

    public void Init(float duration, int damage, float damageInterval, float minSize, float maxSize, GatlingGun gatlingGun) {
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
        gatlingGun.SmallAreas.Remove(this.gameObject);
        Destroy(gameObject);
    }
}