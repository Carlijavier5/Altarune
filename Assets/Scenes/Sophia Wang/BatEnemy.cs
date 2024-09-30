using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BatEnemy : Entity
{
    // Start is called before the first frame update
    //make start braces on the same line as method
    //mark as walkable? not use a navmesh, character
    //add a timer for time between new random movement
    private GameObject _player;
    private Collider _bcollider;
    private bool _aggro = false;
    private float _timer = 0f;
    [SerializeField] private float _timebetweenmove = 1f;
    private Bounds _bounds;
    [SerializeField] private float _speed = 5f;
    private Vector3 _dest;

    void Start(){
        _bcollider = GetComponent<BoxCollider>();
        _dest = GetPosInCollider();
        //dest seems like it's not changing, just 0,0,0
        _bounds = _bcollider.bounds;
    }

    // Update is called once per frame
    protected override void Update(){
        base.Update();
        _player = GameObject.Find("Player");
        Behavior();
    }

    private void Behavior(){
        _timer += Time.deltaTime;
        if (_aggro){
            Vector3 playerPos = new Vector3(_player.transform.position.x, _player.transform.position.y, _player.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, playerPos, _speed*Time.deltaTime);
        }
        else {
            if (_timer >= _timebetweenmove) {
                _dest = GetPosInCollider();
                _timer = 0f;
            }
            Movement(_dest);
        }
    }
    private Vector3 GetPosInCollider(){
        Vector3 randomPos = new Vector3(Random.Range(_bounds.min.x, _bounds.max.x), Random.Range(_bounds.min.y, _bounds.max.y), Random.Range(_bounds.min.z, _bounds.max.z));
        return randomPos;
    }

    private void Movement(Vector3 destination){
        transform.position = Vector3.MoveTowards(transform.position, destination,_speed*Time.deltaTime);
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.GetComponent<Player>() == null){
            return;
        }else{
            _aggro = true;
        }
    }

    private void OnTriggerStay(Collider other){
        if (other.gameObject.GetComponent<Player>() == null){
            return;
        }
        other.gameObject.GetComponent<Player>().TryDamage(1);
        //damage yan?
    }
}
