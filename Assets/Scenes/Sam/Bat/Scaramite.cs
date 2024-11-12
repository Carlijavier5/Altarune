using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Bat Behavior
///    Bat will have 2 states: roam, chase
///    Roam state: Random movements biased towards player with objective to face the player
///    Chase state: when bat can see the player within its primary box collider, it will lock on to the player and chase
///    
/// </summary>

public class Scaramite : Entity {

    [SerializeField] private Collider visionCollider,
                                      bodyCollider;
    [SerializeField] private ScaramiteBodyTrigger attackTrigger;
    [SerializeField] private Rigidbody rb;

    [Header("Movement Variables")]
    [SerializeField] private float linearAcceleration;
    [SerializeField] private float angularSpeed;
    [SerializeField] private float playerBias = 0.65f;

    [Header("Roam Variables")]
    [SerializeField] private float timeBetweenMove = 0.35f;
    [SerializeField] private float roamSpeed = 3.5f;

    [Header("Chase Variables")]
    [SerializeField] private float chaseSpeed = 5f;

    [Header("Damage Variables")]
    [SerializeField] private float damageAmount = 1;
    [SerializeField] private float damageCD = 1f;

    [Header("Self-Knockback Variables")]
    [SerializeField] private float knockbackForce = 2f;
    [SerializeField] private float knockbackDuration = 0.8f;

    private float speed;
    private Vector3 destination;
    private bool isKnockedBack;

    private float timer;
    private float damageCDEndTime;
    private float knockbackEndTime;

    private Player player; //Will store the player that triggered the chase

    private enum State {
        Roam,
        Chase
    }

    private State state = State.Roam;
    
    void Start() {
        destination = GetRoamDestination(); //Begin movement pattern towards the player
    }

    protected override void Update() {
        base.Update();
        if (isKnockedBack) { //Don't want to call move commands if being knocked back
            if (Time.time >= knockbackEndTime) {
                isKnockedBack = false;
                attackTrigger.SetCanAttack(true); //Bat can attack again after knockback
            } else {
                return;
            }
        }
    }

    void FixedUpdate() {
        switch (state) {
            case State.Roam:
                Roam();
                break;
            case State.Chase:
                Chase();
                break;
        }
    }

    //Detects if the player has entered the scaramite's vision and triggers chase mode
    void OnTriggerEnter(Collider other) {
        if (state == State.Chase) return;
        else if (other.TryGetComponent(out Player player)) {
            this.player = player;
            SetState(State.Chase);
        }
    }

    private void SetState(State state) {
        speed = state == State.Chase ? chaseSpeed
                                     : roamSpeed;
        this.state = state;
    }

    //Deals damage to the player
    public void DealDamage(Player player) {
        if (Time.time < damageCDEndTime) return;
        player.TryDamage(1);
        damageCDEndTime = Time.time + damageCD; //Store the time bat dealt damage
        ApplySelfKnockback();
    }

    //Applies knockback to the bat and disables its attacks
    private void ApplySelfKnockback() {
        Vector3 kbDir = (transform.position - player.transform.position).normalized;
        kbDir.y = 0;

        rb.velocity = Vector3.zero; //Reset velocity to prevent movement conflicts
        rb.AddForce(kbDir * knockbackForce, ForceMode.Impulse);

        isKnockedBack = true;
        attackTrigger.SetCanAttack(false); //Make sure the scaramite can't attack while being knocked back

        knockbackEndTime = damageCDEndTime + knockbackDuration; //Store the time when knockback ends
    }


    //Roam mode will have the scaramite move randomly within its vision
    private void Roam() {
        
        if (Vector3.Distance(destination, transform.position) < 0.1f) { //If scaramite is about to reach destination
            DoRotation();
            timer += FixedDeltaTime;
        }

        Vector3 dir = (transform.position - destination).normalized;
        Vector3 forceVector = FixedDeltaTime * linearAcceleration * dir;
        rb.AddRelativeForce(forceVector.x / (speed + 1f), 0f, forceVector.y / (speed + 1f), ForceMode.Acceleration);

        if (timer >= timeBetweenMove) {
            destination = GetRoamDestination(); //Get new destination
            timer = 0f;
        }
    }


    //Chase mode will have the bat move faster and constantly directly towards the player
    private void Chase() {

        transform.position = Vector3.MoveTowards(transform.position, ChasePlayerPosition(), speed * FixedDeltaTime);
    }

    //Rotates the bat to face the player
    //Returns the position of the player
    private Vector3 ChasePlayerPosition() {
        DoRotation();
        return new Vector3(player.transform.position.x,
                           transform.position.y,
                           player.transform.position.z);
    }

    //Returns a random position within the bounds of the bat's vision
    private Vector3 GetRoamDestination() {
        Bounds bounds = visionCollider.bounds;
        return new Vector3(Random.Range(bounds.min.x, bounds.max.x),
                           Random.Range(1.25f, 1.5f),
                           Random.Range(bounds.min.z, bounds.max.z));
    }

    //Rotates the bat 75% of the way towards the player
    private void DoRotation() {
        Vector3 dir = transform.position - player.transform.position;

        dir.y = 0; //Don't rotate on y-axis

        //Make sure the bat isn't going 100% towards the player 
        //(it's actually unplayable otherwise lol)
        dir.x *= playerBias;
        dir.z *= playerBias;

        if (dir != Vector3.zero) {
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                                                  Quaternion.LookRotation(dir),
                                                  DeltaTime * angularSpeed);
        }
    }

    public override void Perish() {
        base.Perish();
        DetachModules();
        enabled = false;
        bodyCollider.enabled = false;
        visionCollider.enabled = false;
        rb.constraints = new();
        rb.isKinematic = false;
        rb.useGravity = true;
        Destroy(gameObject, 2);
    }
}

