using UnityEngine;

public class ProjectileBehavior : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {

        // Check if the bullet hits the player
        if (other.TryGetComponent(out Player player)) {
            Debug.Log("PLAYER DAMAGED");
            player.TryDamage(1);
            Destroy(gameObject); // Destroy projectile on hit
        }
    }

    private void Start() {
        Destroy(gameObject, 3f); // Destroy the projectile after 3 seconds to clean up stray bullets
    }
}
