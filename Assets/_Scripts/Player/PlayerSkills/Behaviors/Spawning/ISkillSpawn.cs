using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillSpawn
{
    BasePlayerSkill SpawnSkill(BasePlayerSkill skillObject, Vector3 playerPos, Vector3 targetPos);
}
