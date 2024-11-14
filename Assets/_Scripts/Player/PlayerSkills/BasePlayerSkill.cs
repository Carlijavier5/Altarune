using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent class for all Player Skills. Extra behaviors can be implemented through skill interfeaces
/// </summary>
public abstract class BasePlayerSkill : MonoBehaviour    // maybe inherit from base object
{
    /// <summary>
    /// Spawns the skill. You can either spawn it at yan, or another point entirely
    /// </summary>
    public virtual void SpawnSkill() {

    }
}
