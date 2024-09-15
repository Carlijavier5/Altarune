using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackEnemy : Entity
{
    protected float height = 0f;
    protected float speed = 1f;
    public Transform target; // The object to face
    public float thrust = 2f;       // Vertical thrust power
    public float moveForce = 50f;    // Forward movement force
    public float maxSpeed = 5f;      // Maximum speed

    public float changeDirectionTime = 3f;  // Time to change direction
    public float flyingDuration = 10f; // Duration of flying before landing
    public float groundedDuration = 5f; // Duration of being grounded before flying again

    public GameObject projectilePrefab; // Prefab of the projectile to shoot
    public float shootingCooldown = 1f; // Cooldown between shots
    public float projectileSpeed = 10f; // Speed of the projectile

    public float maxHeight = 5f; // Maximum flying height
    public float minHeight = 2.5f;
    public float wallAvoidanceDistance = 2f; // Distance to start avoiding walls


    public float oscillateAmplitude = 1f;

    private Rigidbody rb;
    private Vector3 randomDirection;
    private float timer;
    private bool isFlying = true;
    private float stateDurationTimer;
    private float shootingTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetRandomDirection();
        stateDurationTimer = flyingDuration;
        shootingTimer = shootingCooldown;
    }

    protected override void Update()
    {
        base.Update();
        if (isFlying)
        {
            Flying();
        }
        else
        {

            Grounded();
        }

        // Check if it's time to switch states
        stateDurationTimer -= Time.deltaTime;
        if (stateDurationTimer <= 0)
        {
            ToggleFlying();
        }
    }



    void Flying()
    {
        // Make the enemy face the target
        if (target != null)
        {
            transform.LookAt(target);
        }

        // Check for wall avoidance
        if (Physics.Raycast(transform.position, randomDirection, wallAvoidanceDistance))
        {
            SetRandomDirection(); // Change direction if about to hit a wall
        }

        // Apply movement force
        rb.AddForce(randomDirection * moveForce);

        // Limit speed
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        // Apply vertical thrust, but limit the maximum height
        if (transform.position.y < maxHeight && transform.position.x > minHeight)
        {
            rb.AddForce(Vector3.up * Mathf.Sin(Time.time * thrust), ForceMode.Impulse);
        }
        else if (transform.position.y > maxHeight)
        {
            // Apply downward force if at or above max height
            rb.AddForce(Vector3.down * thrust, ForceMode.Impulse);
        }

        else if (transform.position.y < minHeight)
        {
            rb.AddForce(Vector3.up * thrust, ForceMode.Impulse);
        }


        // Change movement direction after a set interval
        timer += Time.deltaTime;
        if (timer > changeDirectionTime)
        {
            SetRandomDirection();
            timer = 0f;
        }
    }

    void Grounded()
    {
        // Ensure the enemy stays on the ground
        rb.useGravity = true;

        // Apply a strong downward force to keep it grounded


        // Face the target
        if (target != null)
        {
            Vector3 directionToTarget = target.position - transform.position;
            directionToTarget.y = 0; // Ignore vertical difference
            transform.rotation = Quaternion.LookRotation(directionToTarget);
        }

        // Attempt to shoot
        shootingTimer -= Time.deltaTime;
        if (shootingTimer <= 0)
        {
            Shoot();
            shootingTimer = shootingCooldown;
        }
    }

    void SetRandomDirection()
    {
        randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    void ToggleFlying()
    {
        isFlying = !isFlying;
        if (isFlying)
        {
            rb.useGravity = false;
            stateDurationTimer = flyingDuration;
        }
        else
        {
            rb.useGravity = true;
            stateDurationTimer = groundedDuration;
            // Apply an immediate downward force when transitioning to grounded state
            rb.AddForce(Vector3.down * 500f, ForceMode.Impulse);
        }
    }

    void Shoot()
    {
        if (target != null && projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward, Quaternion.identity);
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            if (projectileRb != null)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                projectileRb.AddForce(direction * projectileSpeed, ForceMode.Impulse);
            }
        }
    }
}
