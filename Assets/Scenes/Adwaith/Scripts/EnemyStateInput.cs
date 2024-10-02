public class EnemyStateInput : StateInput {
    private EnemyController enemyController;

    public EnemyStateInput(EnemyController enemyController) {
        SetEnemyController(enemyController);
    }

    public void SetEnemyController(EnemyController controller) {
        enemyController = controller;
    }

    public EnemyController GetEnemyController() {
        return enemyController;
    }
}