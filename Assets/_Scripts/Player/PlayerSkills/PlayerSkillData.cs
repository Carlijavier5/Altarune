using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlayerSkillData : ScriptableObject {
    public BasePlayerSkill prefab;
    public Sprite icon;
    public float energyCost;
    public float spawnDistance;
    public float despawnTime;

    public float moveSpeed;
}
