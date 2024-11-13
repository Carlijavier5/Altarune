using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example skill
/// </summary>
public class TestPlayerSkill : BasePlayerSkill 
{
    void Start() {
        SetSpawnBehavior(new SpawnAtDistance());
    }

    public override void SpawnSkill(PlayerSkillData data, Vector3 playerPos, Vector3 targetPos) {
        base.SpawnSkill(data, playerPos, targetPos);
    }
}
