using UnityEngine;

public partial class Snake {
    [SerializeField] public float loiterRadius;

    private class State_Idle : State<Snake_Input> {
        public override void Enter(Snake_Input input) {
            input.snake.navMeshAgent.SetDestination(input.snake.navMeshAgent.transform.position);
        }

        public override void Update(Snake_Input input) {
            if (input.snake.navMeshAgent.remainingDistance > input.snake.loiterRadius) {
                input.stateMachine.SetState(new State_Return());
            }
            if (input.snake.getTarget()){
                input.stateMachine.SetState(new State_Agro());
            }
        }

        public override void Exit(Snake_Input input) { }
    }

    private class State_Return : State<Snake_Input>{
        public override void Enter(Snake_Input input) {
            input.snake.navMeshAgent.SetDestination(input.snake.parent.transform.position);
        }

        public override void Update(Snake_Input input) {
            input.snake.navMeshAgent.SetDestination(input.snake.parent.transform.position);

            if (input.snake.navMeshAgent.remainingDistance <= input.snake.loiterRadius) {
                input.stateMachine.SetState(new State_Idle());
            }
        }

        public override void Exit(Snake_Input input) { }
    }
}