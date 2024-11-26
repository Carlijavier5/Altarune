using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootSkill : BasePlayerSkill {

    public override void SpawnSkill(PlayerSkillData data, Vector3 playerPos, Vector3 targetPos, ISkillSpawn spawnBehavior, ISkillMovement moveBehavior) {
        base.SpawnSkill(data, playerPos, targetPos, new SpawnAtDistance(), new DefaultSkillMovement());
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity)) {
            RootStatusEffect root = new RootStatusEffect(_data.despawnTime);
            entity.ApplyEffects(new[] { root });
        }
    }

    //protected override void DespawnSkill() {
    //    foreach (BaseObject obj in affectedEnemies) {
    //        obj.CanMove = true;
    //    }
    //    Destroy(this.gameObject);
    //}
}
