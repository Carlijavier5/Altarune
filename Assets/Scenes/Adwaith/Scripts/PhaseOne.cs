using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// TryDamage

public class PhaseOne : State<EnemyStateInput> {
    // Creating necessary variables
    private EnemyController enemy;
    private Transform player;
    private NavMeshAgent navigation;
    private Coroutine malfunctionCoroutine;

    private float speed;
    private float stoppingDistance;
    
    private bool spinState = false;
    private readonly float spinDuration = 1;

    private bool runOnce = false;

    // First method to run
    public override void Enter(EnemyStateInput input) {
        // Ensures the method only runs once
        if (runOnce) return;
        runOnce = true;

        // Initializes the enemy with the current EnemyController
        enemy = input.EnemyController;

        // Initializes methods with values from the enemy
        player = enemy.Player;
        navigation = enemy.Navigation;

        // Initializes variables with values from the enemy
        speed = enemy.Speed;
        stoppingDistance = enemy.StoppingDistance;

        // Initializes a Coroutine used for random spinning
        malfunctionCoroutine = enemy.StartCoroutine(MalfunctionCoroutine());
    }

    public override void Update(EnemyStateInput input) {
        // Switches between rotating the enemy and following the player
        if (spinState == true && navigation.remainingDistance != 0 && !navigation.pathPending) {
            enemy.transform.Rotate(Vector3.up, 500f * Time.deltaTime);
        } else {
            FollowPlayer();
        }
    }

    // Method that outlines how the enemy follows the player
    public void FollowPlayer() {
        // Calculates the normalized vector in the direction of the player
        Vector3 directionToPlayer = (player.position - enemy.transform.position).normalized;
        // Calculates the angle between the enemy and the player
        float angleToPlayer = Vector3.Angle(enemy.transform.forward, directionToPlayer);

        // If the angle is less then 20 degrees
        if (angleToPlayer <= 20.0f) {
            navigation.speed = speed;
            navigation.stoppingDistance = stoppingDistance;
            navigation.autoBraking = true;
            navigation.SetDestination(player.position);

            // Logic to ensure the enemy does not move around if the player moves nearby
            if ((navigation.remainingDistance <= navigation.stoppingDistance) && !navigation.pathPending) {
                navigation.isStopped = true;
            } else {
                navigation.isStopped = false;
            }
        } else {
            LookTowardsPlayer();
        }
    }

    // Method that randomly (5 - 10 seconds) calls the SpinCoroutine method
    IEnumerator MalfunctionCoroutine() {
        while(true) {
            yield return new WaitForSeconds(Random.Range(5, 10));
            yield return SpinCoroutine();
        }
    }

    // Method that performs the spin logic
    IEnumerator SpinCoroutine() {
        spinState = true;
        // Spin duration is 1 second
        yield return new WaitForSeconds(spinDuration);
        // Forces the enemy to look at the player before moving
        LookTowardsPlayer();
        spinState = false;
    }

    public void LookTowardsPlayer() {
        // Determines how the enemy should rotate towards the player
        Vector3 directionToPlayer = (player.position - enemy.transform.position).normalized;

        Quaternion rotateToPlayer = Quaternion.LookRotation(directionToPlayer);
        // Rotates the enemy towards the player (uses slerp)
        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, rotateToPlayer, 2f * Time.deltaTime);
    }

    public override void Exit() {
        // Stop the Coroutine
        if (malfunctionCoroutine != null) {
            enemy.StopCoroutine(malfunctionCoroutine);
            malfunctionCoroutine = null;
        }
    }
}
