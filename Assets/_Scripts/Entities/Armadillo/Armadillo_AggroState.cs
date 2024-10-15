using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.AI;

public partial class Armadillo : Entity {
  private record ArmadilloAggroState : ArmadilloState {
    public override void Enter(Armadillo armadillo) {
      armadillo.Agitation = 1.75f;
      armadillo.navMeshAgent.enabled = true;
    }

    public override void Update(Armadillo armadillo) {
      armadillo.navMeshAgent.SetDestination(armadillo.aggroEntity.transform.position);
      if (armadillo.inAggroRange == false) {
        armadillo.SetState(new ArmadilloRoamState());
      }
    }

    public override void Exit(Armadillo armadillo) { armadillo.navMeshAgent.enabled = false; }
  }

}