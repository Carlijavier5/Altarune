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

    // inject behavior logic
    private ISkillSpawn _spawnBehavior;

    /// <summary>
    /// Spawns the skill. You can either spawn it at yan, or another point entirely
    /// </summary>
    public virtual void SpawnSkill(PlayerSkillData data, Vector3 playerPos, Vector3 targetPos) {
        // init the skill data
        this._data = data;
        
        // if no spawn behavior is set
        if (_spawnBehavior == null) { _spawnBehavior = new DefaultSpawn(); }

        _spawnBehavior.SpawnSkill(this.gameObject, playerPos, targetPos);
    }

    public virtual void DespawnSkill() {

    }

    protected void SetSpawnBehavior(ISkillSpawn spawnBehavior) { _spawnBehavior = spawnBehavior; }
}
