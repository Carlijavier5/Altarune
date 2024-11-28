using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Default movement is nothing (static skill)
/// </summary>
public class DefaultSkillMovement : MonoBehaviour, ISkillMovement {
    public void MoveSkill(GameObject skillObject, Vector3 targetPos, float speed) { }
}
