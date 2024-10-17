using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.AI;

public partial class Armadillo : Entity {
  // Struct to store damage event information
  private struct DamageEvent {
    public int damageAmount;
    public float time;

    public DamageEvent(int amount, float time) {
      this.damageAmount = amount;
      this.time = time;
    }
  }

  [Header("Rolling")]
  // List to store the damage events
  private readonly List<DamageEvent> damageEvents = new();

  // Time window to track damage (5 seconds)
  [SerializeField]
  private float damageTrackingWindow = 5f;

  [SerializeField]
  private float rollThresholdDamage = 5;

  [SerializeField]
  public RollStatusEffect rollStatusEffect;


  private void CleanUpOldDamageEvents() {
    float currentTime = Time.time;

    // Remove any damage event that is older than 5 seconds
    damageEvents.RemoveAll(eventData => currentTime - eventData.time > damageTrackingWindow);
  }

  private void IgnoreDamageEvents() {
    damageEvents.Clear();
  }

  public void MaybeRollUp() {
    int damageSum = 0;
    foreach (DamageEvent ev in damageEvents) {
      damageSum += ev.damageAmount;
    }
    if (damageSum > rollThresholdDamage) {
      SetState(new ArmadilloRollState());
    }
  }

  void HandleTryDamage(int amount, ElementType element, EventResponse response) {
    damageEvents.Add(new DamageEvent(amount, Time.time));
  }
  private void AggroRange_OnAggroEnter(Entity entity) {
    inAggroRange = true;
    aggroEntity = entity;
  }

  private void AggroRange_OnAggroExit(Entity entity) {
    inAggroRange = false;
  }

}