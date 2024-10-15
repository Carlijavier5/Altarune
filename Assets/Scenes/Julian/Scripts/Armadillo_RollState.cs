using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.AI;

public partial class Armadillo : Entity {
  private record ArmadilloRollState : ArmadilloState {
    private RollStatusEffect effect;
    private float timeRemaining;
    private Vector3 randomDirection;
    public override void Enter(Armadillo armadillo) {
      timeRemaining = Random.Range(armadillo.rollWaitTimeRange.x, armadillo.rollWaitTimeRange.y) * armadillo.TimeScale;
      effect = armadillo.rollStatusEffect.Clone() as RollStatusEffect;
      armadillo.ApplyEffects(new[] { effect });

      float angle = Random.Range(0f, 360f);  // Random angle in degrees
      randomDirection = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));

      armadillo.navMeshAgent.enabled = true;

      armadillo.rollParticleSystem.Play();
    }

    public override void Update(Armadillo armadillo) {
      timeRemaining -= Time.deltaTime * armadillo.TimeScale;
      if (timeRemaining <= 0) {
        armadillo.SetState(new ArmadilloRoamState());
        return;
      }
      armadillo.navMeshAgent.speed = armadillo.rollSpeed * armadillo.TimeScale;
      armadillo.navMeshAgent.SetDestination(armadillo.transform.position + randomDirection * 10);
      armadillo.IgnoreDamageEvents();
    }

    public override void Exit(Armadillo armadillo) {
      effect.Stop();
      armadillo.UpdateNavMeshSpeeds();
      armadillo.navMeshAgent.enabled = false;
      armadillo.rollParticleSystem.Stop();

    }
  }

}