using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SnakeBehavior : Entity {

    [SerializeField] private float _timeBetweenMove = 0.03f;
    [SerializeField] private float _speed = 2.0f;
    [SerializeField] private float _wiggleAmplitude = 0.5f; //Controls the wiggle intensity
    [SerializeField] private float _wiggleFrequency = 2.0f; //Controls how fast the wiggle occurs

    private float _timer = 0f;
    private Collider _collider;
    private Rigidbody _rb;
    private Vector3 destination;
    private Vector3 moveDirection;

    void Start() {
        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
        destination = GetDestination();
    }

    protected override void Update() {
        base.Update();
        Move();
    }

    //Returns a random position within the bounds of the snake's EMP field
    private Vector3 GetDestination() {
        Bounds bounds = _collider.bounds;
        Vector3 position = new Vector3();

        position.x = Random.Range(bounds.min.x, bounds.max.x);
        position.z = Random.Range(bounds.min.z, bounds.max.z);

        return position;
    }

    private void Move() {
        Vector3 direction = RotateSnake();


        float wiggleOffset = Mathf.Sin(Time.time * _wiggleFrequency) * _wiggleAmplitude; //A*sin(wt) wiggle pattern
        Vector3 wiggleDirection = new Vector3(-direction.z, 0, direction.x) * wiggleOffset; //Perpendicular to main direction
        transform.position = transform.position + (direction + wiggleDirection) * _speed * Time.deltaTime;


        if (Vector3.Distance(destination, transform.position) < 0.1f) {
            _timer += Time.deltaTime;
        }

        if (_timer >= _timeBetweenMove) {
            destination = GetDestination(); //Get new destination
            _timer = 0f;
        }
    }


    private Vector3 RotateSnake() {
        Vector3 direction = (destination - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _speed);
        return direction;

    }
}
