using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example skill 1
/// </summary>
public class TestPlayerSkill2 : BasePlayerSkill 
{
    public override void SpawnSkill(PlayerSkillData data, Vector3 playerPos, Vector3 targetPos, ISkillSpawn ignore, ISkillMovement ignore2) {
        base.SpawnSkill(data, playerPos, targetPos, new SpawnAtDistance(), new DefaultSkillMovement());
    }

    // can override this for custom movement behavior
    //public override void MoveSkill() {
    //    // nothing
    //}
}
