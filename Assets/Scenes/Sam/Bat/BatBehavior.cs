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

public class BatBehavior : Entity {

    private BatBodyTrigger _attackTrigger;
    private Collider _collider;

    private Rigidbody _rb;

    [SerializeField] private float _timeBetweenMove = 0.35f;
    [SerializeField] private float _speed = 3.5f;
    private float _timer = 0f;

    private float _timeDamaged = 0f;
    private float _damageCd = 1f;
    private bool isKnockedBack = false;   //Tracks if the bat is currently knocked back
    private float knockbackDuration = 0.8f;  //Knockback duration
    private float knockbackEndTime = 0f;

    private float kbForce = 2f; //Knockback force

    private Vector3 destination;

    private Player _player; //Will store the player that triggered the chase
    private float bias = 0.65f; //Bias towards the player

    private enum BatState {
        Roam,
        Chase
    }

    private BatState state;
    
    void Start() {
        _attackTrigger = GetComponentInChildren<BatBodyTrigger>();
        _player = FindObjectOfType<Player>();
        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();

        state = BatState.Roam; //Initial state is roam/neutral

        destination = GetDestination(); //Begin movement pattern towards the player
    }

    protected override void Update() {
        base.Update();
        if (isKnockedBack) { //Don't want to call move commands if being knocked back
            if (Time.time >= knockbackEndTime) {
                isKnockedBack = false;
                _attackTrigger.SetCanAttack(true); //Bat can attack again after knockback
            } else {
                return;
            }
        }


        if (state == BatState.Roam) {
            Roam();
        } else {
            Chase();
        }
    }


    //Detects if the player has entered the bat's vision and triggers chase mode
    void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent(out Player _) || state == BatState.Chase) return;

        state = BatState.Chase;
    }

    //Deals damage to the player
    public void DealDamage(Player player) {
        if (Time.time - _timeDamaged < _damageCd) return;
        player.TryDamage(1);
        _timeDamaged = Time.time; //Store the time bat dealt damage
        ApplyKB();
    }

    //Applies knockback to the bat and disables its attacks
    private void ApplyKB() {
        Vector3 kbDir = (transform.position - _player.transform.position).normalized;
        kbDir.y = 0;

        _rb.velocity = Vector3.zero; //Reset velocity to prevent movement conflicts
        _rb.AddForce(kbDir * kbForce, ForceMode.Impulse);

        isKnockedBack = true;
        _attackTrigger.SetCanAttack(false); //Make sure the bat can't attack while being knocked back

        knockbackEndTime = _timeDamaged + knockbackDuration; //Store the time when knockback ends
    }


    //Roam mode will have the bat move randomly within its vision
    private void Roam() {
        
        if (Vector3.Distance(destination, transform.position) < 0.1f) { //If bat is about to reach destination
            RotateBat();
            _timer += Time.deltaTime;
        }
        transform.position = Vector3.MoveTowards(transform.position, destination, _speed * Time.deltaTime);

        if (_timer >= _timeBetweenMove) {
            destination = GetDestination(); //Get new destination
            _timer = 0f;
        }
    }


    //Chase mode will have the bat move faster and constantly directly towards the player
    private void Chase() {
        _speed = 5f;
        transform.position = Vector3.MoveTowards(transform.position, ChasePlayerPosition(), _speed * Time.deltaTime);
    }

    //Rotates the bat to face the player
    //Returns the position of the player
    private Vector3 ChasePlayerPosition() {
        Vector3 res = new Vector3();
        RotateBat();
        res.x = _player.transform.position.x;
        res.y = transform.position.y; //Keep the same height
        res.z = _player.transform.position.z;
        return res;
    }

    //Returns a random position within the bounds of the bat's vision
    private Vector3 GetDestination() {
        Bounds bounds = _collider.bounds;
        Vector3 position = new Vector3();

        position.x = Random.Range(bounds.min.x, bounds.max.x);
        position.y = Random.Range(1.25f, 1.5f); //Allow for wiggle room in y-axis to simulate flight
        position.z = Random.Range(bounds.min.z, bounds.max.z);

        return position;
    }

    //Rotates the bat 75% of the way towards the player
    private void RotateBat() {
        Vector3 dir = transform.position - _player.transform.position;

        dir.y = 0; //Don't rotate on y-axis

        //Make sure the bat isn't going 100% towards the player 
        //(it's actually unplayable otherwise lol)
        dir.x *= bias;
        dir.z *= bias;

        if (dir != Vector3.zero) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
        }

    }

    public override void Perish() {
        base.Perish();
        DetachModules();
        enabled = false;
        _rb.constraints = new();
        _rb.isKinematic = false;
        _rb.useGravity = true;
        Destroy(gameObject, 2);
    }
}

