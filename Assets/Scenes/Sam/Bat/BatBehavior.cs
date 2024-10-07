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
///    Chase state: when bat can see the player within range x, it will lock on to the player and chase
///    
/// </summary>

public class BatBehavior : MonoBehaviour
{

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

        destination = getDestination(); //Begin movement pattern towards the player
    }

    void Update() {
        if (isKnockedBack) { //Don't want to call move commands if being knocked back
            if (Time.time >= knockbackEndTime) {
                isKnockedBack = false;
                _attackTrigger.setCanAttack(true);
            } else {
                return;
            }
        }


        if (state == BatState.Roam) {
            roam();
        } else {
            chase();
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<Player>() == null || state == BatState.Chase) return;

        state = BatState.Chase;
    }

    public void dealDamage() {
        if (Time.time - _timeDamaged < _damageCd) return;
        Debug.Log("DAMAGE DEALT!");
        _timeDamaged = Time.time;
        applyKB();
    }

    private void applyKB() {
        Vector3 kbDir = (transform.position - _player.transform.position).normalized;
        kbDir.y = 0;

        _rb.AddForce(kbDir * kbForce, ForceMode.Impulse);

        isKnockedBack = true;
        _attackTrigger.setCanAttack(false);

        knockbackEndTime = Time.time + knockbackDuration;
    }



    private void roam() {

        if (Vector3.Distance(destination, transform.position) < 0.1f) { //If bat is about to reach destination
            rotateBat();
            _timer += Time.deltaTime;
        }

        transform.position = Vector3.MoveTowards(transform.position, destination, _speed * Time.deltaTime);

        if (_timer >= _timeBetweenMove) {
            destination = getDestination(); //Get new destination
            _timer = 0f;
        }
    }

    private void chase() {
        _speed = 5f;
        transform.position = Vector3.MoveTowards(transform.position, chasePlayerPosition(), _speed * Time.deltaTime);
    }

    private Vector3 chasePlayerPosition() {
        Vector3 res = new Vector3();
        rotateBat();
        res.x = _player.transform.position.x;
        res.y = transform.position.y; //Keep the same height
        res.z = _player.transform.position.z;
        return res;
    }


    private Vector3 getDestination() {
        Bounds bounds = _collider.bounds;
        Vector3 position = new Vector3();

        position.x = Random.Range(bounds.min.x, bounds.max.x);
        position.y = Random.Range(1.25f, 1.5f);
        position.z = Random.Range(bounds.min.z, bounds.max.z);

        return position;
    }

    private void rotateBat() {
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
}

