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
    [SerializeField] private float _timecc = 3f;
    private Bounds _bounds;
    [SerializeField] private float _speed = 3f;
    private Vector3 _dest;
    private bool canMove = true;
    private bool canHurt = true;
    void Awake()
    {
        OnStunSet += Bat_Stun;
        OnTimeScaleSet += Bat_Time;
        OnRootSet += Bat_Root;
    }
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
    private void Bat_Stun(bool stunned){
        canMove = false;
        canHurt = false;
    }
    private void Bat_Time(float timeScale){
        _speed *= timeScale;
    }
    private void Bat_Root(bool rooted){
        canMove = false;
    }
    private void Behavior(){
        _timer += Time.deltaTime;
        if (canMove){
            if (_aggro)
            {
                Vector3 playerPos = new Vector3(_player.transform.position.x, _player.transform.position.y, _player.transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, playerPos, _speed * Time.deltaTime);
            }
            else
            {
                if (_timer >= _timebetweenmove)
                {
                    _dest = GetPosInCollider();
                    _timer = 0f;
                }
                Movement(_dest);
            }
        } else {
            _speed = 0f;
            if (_timer >= _timecc){
                _speed = 3f;
                canMove = true;
                canHurt = true;
                _timer = 0f;
            }
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
        if (canHurt){
            other.gameObject.GetComponent<Player>().TryDamage(1);
        }
        //damage yan?
    }
    public override void Perish(bool immediate) {
        base.Perish(immediate);
        if (immediate) Destroy(gameObject);
        else Destroy(gameObject, 1);
    }
}
