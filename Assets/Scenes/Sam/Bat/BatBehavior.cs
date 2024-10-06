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
    private Collider _collider;

    [SerializeField] private float _timeBetweenMove = 0.35f;
    [SerializeField] private float _speed = 3.5f;
    private float _timer = 0f;

    private Vector3 destination;

    private Player _player; //Will store the player that triggered the chase

    private float bias = 0.65f; //Bias towards the player

    private enum BatState {
        Roam,
        Chase
    }

    private BatState state;
    
    void Start() {
        _player = FindObjectOfType<Player>();
        _collider = GetComponent<Collider>();

        state = BatState.Roam; //Initial state is roam/neutral

        destination = getDestination(); //Begin movement pattern towards the player
    }

    void Update() {
        if (state == BatState.Roam) {
            roam();
        } else if (state == BatState.Chase && _player != null) {
            chase();
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<Player>() == null) return;

        state = BatState.Chase;
        Debug.Log("CHASE!!!!!");
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.GetComponent<Player>() == null) return;

        state = BatState.Chase;
        Debug.Log("KILL!!!");
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

