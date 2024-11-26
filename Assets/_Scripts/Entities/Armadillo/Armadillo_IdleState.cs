using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.AI;

public partial class Armadillo : Entity {
  private record ArmadilloIdleState : ArmadilloState {
    private float timeRemaining = 0;
    public override void Enter(Armadillo armadillo) {
      timeRemaining = Random.Range(armadillo.idleWaitTimeRange.x, armadillo.idleWaitTimeRange.y) / armadillo.Agitation * armadillo.TimeScale;
    }

    public override void Update(Armadillo armadillo) {
      armadillo.CalmDown();
      timeRemaining -= armadillo.DeltaTime;


      if (armadillo.inAggroRange) {
        armadillo.SetState(new ArmadilloAggroState());
        return;
      }

      if (timeRemaining <= 0) {
        armadillo.SetState(new ArmadilloRoamState());
        return;
      }
      armadillo.MaybeRollUp();
    }

    public override void Exit(Armadillo armadillo) { }

  }
}