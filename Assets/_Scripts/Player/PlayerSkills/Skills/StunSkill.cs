using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunSkill : BasePlayerSkill
{
    [Header("Stun Parameters")]
    [SerializeField] private float pushStrength;

    public override void SpawnSkill(PlayerSkillData data, Vector3 playerPos, Vector3 targetPos, ISkillSpawn spawnBehavior, ISkillMovement moveBehavior) {
        base.SpawnSkill(data, playerPos, targetPos, new DefaultSpawn(), new ProjectileMovement());
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity)) {
            StunStatusEffect stun = new StunStatusEffect(_data.despawnTime);
            ApplyPushForce(entity);
            entity.ApplyEffects(new[] { stun });
        }
    }

    private void ApplyPushForce(BaseObject obj) {
        Vector3 directionToCenter = (obj.transform.position - transform.position).normalized;
        directionToCenter.y = 0;
        directionToCenter = directionToCenter.normalized;

        obj.TryLongPush(directionToCenter, pushStrength, _data.despawnTime);
    }
}
