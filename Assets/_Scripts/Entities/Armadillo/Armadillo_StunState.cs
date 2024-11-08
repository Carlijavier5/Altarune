using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.AI;

public partial class Armadillo : Entity {
  private record ArmadilloStunState : ArmadilloState {
    public override void Enter(Armadillo armadillo) {
    }

    public override void Update(Armadillo armadillo) {
    }

    public override void Exit(Armadillo armadillo) {
    }
  }

}