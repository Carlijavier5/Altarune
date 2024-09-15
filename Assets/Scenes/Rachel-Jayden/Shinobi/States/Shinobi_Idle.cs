using UnityEngine;

public partial class Shinobi
{
    public class State_Idle : State<Shinobi_Input>
    {

        public override void Enter(Shinobi_Input input)
        {
            Debug.Log("idle");
            input.shinobi.navMeshAgent.ResetPath();

            input.shinobi.Wait();
        }

        public override void Update(Shinobi_Input input) {   }

        public override void Exit(Shinobi_Input input) {   }
    }
}
