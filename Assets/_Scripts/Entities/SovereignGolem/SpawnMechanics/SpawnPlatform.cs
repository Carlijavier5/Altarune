using UnityEngine;

public class SpawnPlatform : MonoBehaviour {

    public event System.Action<SpawnPlatform> OnCrystalShatter;

    [SerializeField] private PositionRandomizer randomizer;
    public bool HasCrystal => activeEntity;

    private Entity activeEntity;

    public void SpawnEntity(Entity entity) {
        activeEntity = Instantiate(entity, transform);
        activeEntity.OnPerish += ActiveEntity_OnPerish;
        randomizer.Toggle(false);
    }

    private void ActiveEntity_OnPerish(BaseObject _) {
        activeEntity = null;
        randomizer.Toggle(true);
        OnCrystalShatter?.Invoke(this);
    }
}