using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public partial class Shinobi
{
    public class State_Idle : State<Shinobi_Input>
    {

        public override void Enter(Shinobi_Input input)
        {
            input.shinobi.navMeshAgent.ResetPath();
        }

        public override void Update(Shinobi_Input input)
        {
            input.shinobi.Wait();

            input.shinobi.stateMachine.SetState(new State_Follow());
        }

        public override void Exit(Shinobi_Input input) {   }
    }
}
