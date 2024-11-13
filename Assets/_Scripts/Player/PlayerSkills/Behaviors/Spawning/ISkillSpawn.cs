using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillSpawn
{
    void SpawnSkill(GameObject skillObject, Vector3 playerPos, Vector3 targetPos);
}
