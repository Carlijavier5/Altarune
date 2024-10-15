using UnityEngine;

public partial class Snake {
    [SerializeField] private int damage;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackTime;
    [SerializeField] private float attackDistance;
    

    private class State_Agro : State<Snake_Input> {
        public override void Enter(Snake_Input input) {
            if(input.snake.target == null) {
                input.stateMachine.SetState(new State_Return());
                return;
            }
            input.snake.navMeshAgent.SetDestination(input.snake.target.transform.position);
        }

        public override void Update(Snake_Input input) {
            if (!input.snake.getTarget()){
                input.snake.target = null;
                input.stateMachine.SetState(new State_Return());
                return;
            } else {
                input.snake.navMeshAgent.SetDestination(input.snake.target.transform.position);
            }

            if (input.snake.navMeshAgent.remainingDistance <= input.snake.attackDistance) {
                input.stateMachine.SetState(new State_Attack());
            }
        }

        public override void Exit(Snake_Input input) { }
    }

    private class State_Attack : State<Snake_Input> {
        private float attackTimer = 0;

        public override void Enter(Snake_Input input) {
            Collider[] hitColliders = Physics.OverlapSphere(input.snake.navMeshAgent.transform.position, input.snake.attackDistance);
            foreach (var hitCollider in hitColliders)
            {
                if(hitCollider.TryGetComponent(out Entity entity)){
                    if (entity.Faction == EntityFaction.Hostile){
                        entity.TryDamage(input.snake.damage);
                    }
                }
            }

            input.snake.navMeshAgent.SetDestination(input.snake.transform.position);
        }

        public override void Update(Snake_Input input) {
            attackTimer += Time.deltaTime;
            if (attackTimer > input.snake.attackTime){
                input.stateMachine.SetState(new State_AttackWait());
            }
        }

        public override void Exit(Snake_Input input) { }
    }

    private class State_AttackWait : State<Snake_Input> {
        private float attackCooldownTimer = 0;

        public override void Enter(Snake_Input input) {
            if(input.snake.target == null) return;
            input.snake.navMeshAgent.SetDestination(input.snake.target.transform.position);
        }

        public override void Update(Snake_Input input) {
            if (!input.snake.getTarget()){
                input.stateMachine.SetState(new State_Agro());
                return;
            }
            input.snake.navMeshAgent.SetDestination(input.snake.target.transform.position);
            attackCooldownTimer += Time.deltaTime;
            if (attackCooldownTimer > input.snake.attackCooldown){
                input.stateMachine.SetState(new State_Attack());
            }
        }

        public override void Exit(Snake_Input input) { }
    }
}