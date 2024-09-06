using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Shinobi
{
    public class State_Aggro : State<Shinobi_Input>
    {
        private Entity _aggroTarget;

        public override void Enter(Shinobi_Input input)
        {
            Debug.Log("aggro");

            _aggroTarget = input.aggroTarget;
        }

        public override void Update(Shinobi_Input input)
        {
            Transform t = input.shinobi.transform;
            Quaternion targetRotation = Quaternion.LookRotation(_aggroTarget.transform.position - t.position, Vector3.up);
            t.rotation = Quaternion.RotateTowards(t.rotation, targetRotation, Time.deltaTime * input.shinobi.navMeshAgent.angularSpeed);
        }

        public override void Exit(Shinobi_Input input)
        {
            // stuff
        }
    }
}
