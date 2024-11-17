using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Always spawn a certain distance from the player
/// </summary>
public class SpawnAtDistance : MonoBehaviour, ISkillSpawn {
    public BasePlayerSkill SpawnSkill(BasePlayerSkill skillObject, Vector3 playerPos, Vector3 targetPos) {
        return Instantiate(skillObject, targetPos, Quaternion.identity);
    }
}
