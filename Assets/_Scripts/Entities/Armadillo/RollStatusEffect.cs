using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

[System.Serializable]
public class RollStatusEffect : StatusEffect {
  [SerializeField]
  private float defenseAddMod;


  private bool stop = false;

  [SerializeField] private Material material;
  public override void Apply(Entity entity, bool isNew) {
    HealthModifiers = new() { defense = { addMod = defenseAddMod } };
  }

  public override void Terminate(Entity entity) { }

  public void Stop() {
    stop = true;
  }

  public override bool Update(Entity entity) {
    return stop;
  }
}