using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.AI;

public partial class Armadillo : Entity {
  private record ArmadilloRoamState : ArmadilloState {
    public override void Enter(Armadillo armadillo) {
      armadillo.navMeshAgent.enabled = true;
      armadillo.MotionDriver.Set(armadillo.navMeshAgent);
      SetNewRoamTarget(armadillo);
    }
    public override void Update(Armadillo armadillo) {
      armadillo.CalmDown();
      if (armadillo.navMeshAgent.remainingDistance <= Mathf.Epsilon) {
        armadillo.SetState(new ArmadilloIdleState());
      }
      armadillo.MaybeRollUp();
    }
    private void SetNewRoamTarget(Armadillo armadillo) {

      Transform t = armadillo.transform;
      Vector3 dir = new(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
      Vector2 distanceRange = armadillo.roamDistanceRange;
      float distance = Random.Range(distanceRange.x, distanceRange.y) * armadillo.Agitation;

      int depthLimit = 0;
      Vector3 targetLocation = Vector3.zero;

      while (targetLocation == Vector3.zero && depthLimit < 10) {
        Ray ray = new(t.position, dir);
        IEnumerable<RaycastHit> info = Physics.RaycastAll(ray, distance + armadillo.roamWallBuffer)
                                              .Where((info) => !info.collider.isTrigger);
        if (info.Count() == 0) {
          targetLocation = ray.GetPoint(distance);
        }
        depthLimit++;
      }
      if (depthLimit >= 10) {
        armadillo.SetState(new ArmadilloIdleState());
        return;
      }
      armadillo.navMeshAgent.SetDestination(targetLocation);
    }
    public override void Exit(Armadillo armadillo) {
      armadillo.navMeshAgent.enabled = false;
      armadillo.MotionDriver.Set(armadillo.transform);
    }
  }
}