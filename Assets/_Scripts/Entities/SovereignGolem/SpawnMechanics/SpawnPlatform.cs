using UnityEngine;

public class SpawnPlatform : MonoBehaviour {

    public event System.Action<SpawnPlatform> OnSpawnPerish;

    [SerializeField] private PositionRandomizer randomizer;
    public bool HasEntity => activeEntity;

    private Entity activeEntity;

    public void SpawnEntity(Entity entity) {
        activeEntity = Instantiate(entity, transform);
        activeEntity.OnPerish += ActiveEntity_OnPerish;
        randomizer.Toggle(false);
    }

    public void CollapseSpawn() {
        if (HasEntity) activeEntity.Perish();
    }

    private void ActiveEntity_OnPerish(BaseObject _) {
        activeEntity = null;
        randomizer.Toggle(true);
        OnSpawnPerish?.Invoke(this);
    }
}