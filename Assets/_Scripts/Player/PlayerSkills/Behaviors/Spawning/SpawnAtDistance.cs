using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Always spawn a certain distance from the player
/// </summary>
public class SpawnAtDistance : MonoBehaviour, ISkillSpawn {
    public void SpawnSkill(GameObject skillObject, Vector3 playerPos, Vector3 targetPos) {
        Instantiate(skillObject, targetPos, Quaternion.identity);
    }
}
