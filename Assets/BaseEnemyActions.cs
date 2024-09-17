public interface IEnemyActions {
    void Enter(EnemyController enemy);
    void Execute();
    void Attack();
    void Defense();
    void FollowPlayer();
    void Exit();
}
