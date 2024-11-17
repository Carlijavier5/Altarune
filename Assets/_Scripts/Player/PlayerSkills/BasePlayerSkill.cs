using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent class for all Player Skills. Extra behaviors can be implemented through skill interfaces
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public abstract class BasePlayerSkill : MonoBehaviour    // maybe inherit from base object
{
    protected PlayerSkillData _data;

    private Vector3 _targetPos;

    // inject behavior logic
    private ISkillMovement _movementBehavior;

    protected void InitSkill(PlayerSkillData data, Vector3 targetPos, ISkillMovement move) {
        _data = data;
        _targetPos = targetPos;
        _movementBehavior = move;
        Invoke("DespawnSkill", _data.despawnTime);

        if (_movementBehavior == null) { _movementBehavior = new DefaultSkillMovement(); }
    }

    void Update() { MoveSkill(); }

    /// <summary>
    /// Spawns the skill. You can either spawn it at yan, or another point entirely
    /// You can override this method entirely if you want your own spawn behavior
    /// </summary>
    public virtual void SpawnSkill(PlayerSkillData data, Vector3 playerPos, Vector3 targetPos, ISkillSpawn spawnBehavior, ISkillMovement moveBehavior) {

        // if no spawn behavior is set
        if (spawnBehavior == null) { spawnBehavior = new DefaultSpawn(); }

        BasePlayerSkill skill = spawnBehavior.SpawnSkill(data.prefab, playerPos, targetPos);
        skill.InitSkill(data, targetPos, moveBehavior);
    }

    public virtual void MoveSkill() {
        _movementBehavior.MoveSkill(this.gameObject, _targetPos, _data.moveSpeed);
    }

    protected virtual void DespawnSkill() {
        Destroy(this.gameObject);
    }

}
