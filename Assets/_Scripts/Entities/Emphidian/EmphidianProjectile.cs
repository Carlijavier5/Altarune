using UnityEngine;

public class EmphidianProjectile : MonoBehaviour {

    [SerializeField] private int damageAmount;
    [SerializeField] private float lifetime;

    void Awake() {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out BaseObject baseObject)
            && baseObject.IsFaction(EntityFaction.Friendly)) {
            baseObject.TryDamage(damageAmount);
            Destroy(gameObject);
        }
    }
}