using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Bat Behavior
///    Bat will have 2 states: roam, chase
///    Roam state: Random movements slightly biased towards player with objective to face the player
///    Chase state: when bat can see the player within range x, it will lock on to the player and chase
///    
/// </summary>

public class BatBehavior : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Collider _collider;



    [SerializeField] private float _timeBetweenMove;
    private float _timer = 0f;


    private enum BatState {
        Roam,
        Chase
    }

    private BatState state;
    
    void Start() {
        _agent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<Collider>();
        state = BatState.Roam;

        _agent.SetDestination(getPositionInCollider());
        
    }

    
    void Update() {

        if (state == BatState.Roam) {
            roam();
        } else {
            //chase();
        }
        
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<Player>() == null) return;

        state = BatState.Chase;
        Debug.Log("CHASE!!!!!");
    }


    private void roam() {

        if (_agent.velocity == Vector3.zero) {
            _timer += Time.deltaTime;
        }
        if (_timer >= _timeBetweenMove) {
            move();
        }

    }

    private void move() {
        _agent.SetDestination(getPositionInCollider());
        _timer = 0f;
    }




    private Vector3 getPositionInCollider() {
        Bounds bounds = _collider.bounds;

        Vector3 position = new Vector3();
        position.x = Random.Range(bounds.min.x, bounds.max.x);
        position.y = Random.Range(bounds.min.y, bounds.max.y);
        position.z = Random.Range(bounds.min.z, bounds.max.z);
        return position;
    }
}
