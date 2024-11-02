using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Necromancer Enemy
/// </summary>
public class Necromancer : MonoBehaviour
{
    /// <summary>
    /// Go over a few conventions like having not having brackets on newline, no public variables, and _ for private vars
    /// </summary>
    private NavMeshAgent _agent;
    private Collider _collider;

    private bool _isAggro = false;

    [SerializeField] private float _timeBetweenMove;
    [SerializeField] private float _timeBetweenMagicBall;
    private float _timer = 0f;

    void Start() {
        _collider = GetComponent<Collider>();
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        SimpleBehavior();
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<Player>() == null) { return; }

        _isAggro = true;
        _timer = 0;
        Debug.Log("i see you YAN");

        // stop here
    }

    /// <summary>
    /// There's a more complex way to do this with state machines but for now we will use if/else
    /// </summary>
    private void SimpleBehavior() {
        // i don't know this stuff on the top of my head, i use the ~api~
        if (_agent.velocity == Vector3.zero) { _timer += Time.deltaTime; }

        if (_isAggro) {

        } else {
            if (_timer >= _timeBetweenMove) {
                Move();
            }
        }
    }

    private void Move() { 
        _agent.SetDestination(GetRandomPosition());
        _timer = 0;
    }

    private Vector3 GetRandomPosition() {
        Bounds bounds = _collider.bounds;

        Vector3 randomPosition = new Vector3 (
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );

        return randomPosition;
    }
}
