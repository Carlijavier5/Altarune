using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shinobi_SweepHitbox : MonoBehaviour
{
    [SerializeField] private float sweepDuration = 0.7f;

    private float elapsedTime;

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= sweepDuration)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Entity entity) && entity.Faction != EntityFaction.Hostile)
        {
            entity.TryDamage(4);
        }
    }
}
