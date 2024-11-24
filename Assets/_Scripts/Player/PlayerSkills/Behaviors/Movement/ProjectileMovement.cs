using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves from yan to a target point at a set speed
/// </summary>
public class ProjectileMovement : MonoBehaviour, ISkillMovement {
    public void MoveSkill(GameObject skillObject, Vector3 targetPos, float speed) {
        Rigidbody rb = skillObject.GetComponent<Rigidbody>();
        Vector3 direction = (targetPos - skillObject.transform.position);
        direction.y = 0f;

        //direction = direction.normalized;
        //rb.velocity = direction * 10f;
        if (direction.magnitude < 0.1f) {
            rb.velocity = Vector3.zero;
        } else {
            direction = direction.normalized;
            rb.velocity = direction * speed;
        }
    }
}
