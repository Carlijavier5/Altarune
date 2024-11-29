using System.Collections;
using UnityEngine;

public class Emphidian : Entity {

    [SerializeField] private float timeBetweenMove = 0.03f;
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float wiggleAmplitude = 0.5f; // Controls the wiggle intensity
    [SerializeField] private float wiggleFrequency = 2.0f; // Controls how fast the wiggle occurs
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint; // Gun barrel
    [SerializeField] private float shootInterval = 3.7f; // Time between shots
    [SerializeField] private float projectileSpeed = 5.0f; // Speed of the projectile
    [SerializeField] private int damage = 1; // Damage dealt by the projectile
    [SerializeField] private float stopDuration = 1.0f; // Time the snake stops to shoot
    [SerializeField] private float playerBias = 1.0f;

    private float moveTimer = 0f;
    private float shootTimer = 0f;
    private Collider coll;
    private Vector3 destination;
    private bool isShooting = false;
    private Player player;

    void Awake() {
        coll = GetComponent<Collider>();
        player = FindObjectOfType<Player>();
        destination = GetDestination();
    }

    protected override void Update() {
        base.Update();

        if (isShooting) return; // Stop when shooting

        Move();

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval) {
            StartShooting(); // Begin shooting immediately
            shootTimer = 0f;
        }
    }

    private void StartShooting() {
        isShooting = true; // Stop moving
        ShootProjectile(); // Shoot immediately
        StartCoroutine(ResumeMovementAfterShoot());
    }

    private IEnumerator ResumeMovementAfterShoot() {
        yield return new WaitForSeconds(stopDuration); // PAUSE!
        isShooting = false; // RESUME!
    }

    private void ShootProjectile() {

        // Rotate snake to face the player before shooting
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = lookRotation;

        // Spawn bullet
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);

        if (projectile == null) {
            Debug.LogError("Failed to instantiate projectile.");
            return;
        }

        // Set bullet velocity toward the player
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        if (projectileRb != null) {
            projectileRb.velocity = directionToPlayer * projectileSpeed;
        }


        Collider projectileCollider = projectile.GetComponent<Collider>();
        Collider snakeCollider = GetComponent<Collider>();

        // Snake collider kept nullifying the projectile behavior .-.
        if (projectileCollider != null && snakeCollider != null) {
            Physics.IgnoreCollision(projectileCollider, snakeCollider);
        }

        // Debug.Log($"Snake fired at player with direction: {directionToPlayer}");
    }

    private Vector3 GetDestination() {
        Bounds bounds = coll.bounds;
        Vector3 randomPosition = new Vector3();

        randomPosition.x = Random.Range(bounds.min.x, bounds.max.x);
        randomPosition.z = Random.Range(bounds.min.z, bounds.max.z);

        // 78% bias to player
        if (player != null) {
            Vector3 playerPosition = player.transform.position;
            randomPosition = Vector3.Lerp(randomPosition, playerPosition, 0.78f); 
        }

        return randomPosition;
    }

    private void Move() {
        Vector3 direction = RotateSnake();

        float wiggleOffset = Mathf.Sin(Time.time * wiggleFrequency) * wiggleAmplitude; // A*sin(wt) wiggle pattern
        Vector3 wiggleDirection = new Vector3(-direction.z, 0, direction.x) * wiggleOffset; // Perpendicular to main direction
        transform.position = transform.position + (direction + wiggleDirection) * speed * Time.deltaTime;

        if (Vector3.Distance(destination, transform.position) < 0.1f) {
            moveTimer += Time.deltaTime;
        }

        if (moveTimer >= timeBetweenMove) {
            destination = GetDestination(); // Get new destination
            moveTimer = 0f;
        }
    }

    private Vector3 RotateSnake() {
        Vector3 direction = (destination - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
        return direction;
    }
}