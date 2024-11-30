using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns the skill at yan's coords
/// </summary>
public class DefaultSpawn : MonoBehaviour, ISkillSpawn 
{
    public BasePlayerSkill SpawnSkill(BasePlayerSkill skillObject, Vector3 playerPos, Vector3 targetPos) {
        return Instantiate(skillObject, playerPos, Quaternion.identity);
    }
}
