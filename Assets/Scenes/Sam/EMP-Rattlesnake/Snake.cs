using System.Collections;
using UnityEngine;

public class SnakeBehavior : Entity {
    [SerializeField] private float _timeBetweenMove = 0.03f;
    [SerializeField] private float _speed = 2.0f;
    [SerializeField] private float _wiggleAmplitude = 0.5f; // Controls the wiggle intensity
    [SerializeField] private float _wiggleFrequency = 2.0f; // Controls how fast the wiggle occurs
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _shootPoint; // Gun barrel
    [SerializeField] private float _shootInterval = 3.7f; // Time between shots
    [SerializeField] private float _projectileSpeed = 5.0f; // Speed of the projectile
    [SerializeField] private int _damage = 1; // Damage dealt by the projectile
    [SerializeField] private float _stopDuration = 1.0f; // Time the snake stops to shoot
    [SerializeField] private float _playerBias = 1.0f;

    private float _moveTimer = 0f;
    private float _shootTimer = 0f;
    private Collider _collider;
    private Rigidbody _rb;
    private Vector3 destination;
    private bool _isShooting = false;
    private Player _player;

    void Start() {
        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
        _player = FindObjectOfType<Player>();
        destination = GetDestination();
    }

    protected override void Update() {
        base.Update();

        if (_isShooting) return; // Stop when shooting

        Move();

        _shootTimer += Time.deltaTime;
        if (_shootTimer >= _shootInterval) {
            StartShooting(); // Begin shooting immediately
            _shootTimer = 0f;
        }
    }

    private void StartShooting() {
        _isShooting = true; // Stop moving
        ShootProjectile(); // Shoot immediately
        StartCoroutine(ResumeMovementAfterShoot());
    }

    private IEnumerator ResumeMovementAfterShoot() {
        yield return new WaitForSeconds(_stopDuration); // PAUSE!
        _isShooting = false; // RESUME!
    }

    private void ShootProjectile() {

        // Rotate snake to face the player before shooting
        Vector3 directionToPlayer = (_player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = lookRotation;

        // Spawn bullet
        GameObject projectile = Instantiate(_projectilePrefab, _shootPoint.position, _shootPoint.rotation);

        if (projectile == null) {
            Debug.LogError("Failed to instantiate projectile.");
            return;
        }

        // Set bullet velocity toward the player
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        if (projectileRb != null) {
            projectileRb.velocity = directionToPlayer * _projectileSpeed;
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
        Bounds bounds = _collider.bounds;
        Vector3 randomPosition = new Vector3();

        randomPosition.x = Random.Range(bounds.min.x, bounds.max.x);
        randomPosition.z = Random.Range(bounds.min.z, bounds.max.z);

        // 78% bias to player
        if (_player != null) {
            Vector3 playerPosition = _player.transform.position;
            randomPosition = Vector3.Lerp(randomPosition, playerPosition, 0.78f); 
        }

        return randomPosition;
    }

    private void Move() {
        Vector3 direction = RotateSnake();

        float wiggleOffset = Mathf.Sin(Time.time * _wiggleFrequency) * _wiggleAmplitude; // A*sin(wt) wiggle pattern
        Vector3 wiggleDirection = new Vector3(-direction.z, 0, direction.x) * wiggleOffset; // Perpendicular to main direction
        transform.position = transform.position + (direction + wiggleDirection) * _speed * Time.deltaTime;

        if (Vector3.Distance(destination, transform.position) < 0.1f) {
            _moveTimer += Time.deltaTime;
        }

        if (_moveTimer >= _timeBetweenMove) {
            destination = GetDestination(); // Get new destination
            _moveTimer = 0f;
        }
    }

    private Vector3 RotateSnake() {
        Vector3 direction = (destination - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _speed);
        return direction;
    }
}
