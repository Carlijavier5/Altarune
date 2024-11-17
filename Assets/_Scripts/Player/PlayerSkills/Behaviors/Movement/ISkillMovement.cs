using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Default behavior of skill projectiles. Can be overriden.
/// </summary>
public interface ISkillMovement
{
    void MoveSkill(GameObject skillObject, Vector3 targetPos, float speed);
}
