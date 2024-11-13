using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandPit : MonoBehaviour
{
    /*
     Intended Behavior: A slow moving sandpit which applies a small pull effect into itself and deals damage to those in the center
     There are 3 major sections of the circle
     It should move very slowly. It does NOT do damage.
     Outer most: A minor pull towards the center which can be resisted.
     Middle most: A strong pull which is stronger than yan's running speed. Is outpaced by yan's dodge
     Center: Super strong pull which requires lots of dodging by Yan.
    */

    [SerializeField] private float moveSpeed;
    [SerializeField] private float updateTime;
    [SerializeField] private float suckUpdateTime;

    private Transform player;
    private float updateClock;
    private float suckUpdateClock;
    private HashSet<Entity> suckTargets = new();

    // Start is called before the first frame update
    void Start()
    {
        FindPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Suck();
    }
    
    private void Move(){
        updateClock += Time.deltaTime;
        if (player != null && updateClock > updateTime) {
            Vector3 difference = Vector3.Normalize(player.position - this.transform.position) * moveSpeed;
            
            this.transform.position += difference;
            updateClock = 0;
        }
    }
    private void Suck(){
        suckUpdateClock += Time.deltaTime;
        if (suckUpdateClock > suckUpdateTime) {
            suckUpdateClock = 0;
            foreach (Entity i in suckTargets) {
                Vector3 difference = Vector3.Normalize(this.transform.position - player.transform.position) * moveSpeed;
            
                player.position += difference;
                updateClock = 0;
            }
        }
    }

    void OnTriggerEnter(Collider other){
        if(other.TryGetComponent(out Entity entity) && 
        entity.Faction == EntityFaction.Friendly){
            Debug.Log("I'm touching " + entity);
            suckTargets.Add(entity);
        }
    }


    
    private bool FindPlayer() {
        // Initializes the OverlapSphere collider
        Collider[] findPlayerCollider = Physics.OverlapSphere(
            transform.position, 
            10f, 
            LayerMask.GetMask("Player")
        );

        bool foundPlayer = findPlayerCollider != null && findPlayerCollider.Length > 0;

        // If the player collider is found, assigns it to the transform
        if (foundPlayer) {
            player = findPlayerCollider[0].transform;
        } else {
            StartCoroutine(AwaitFindPlayer());
        }
        return foundPlayer;
    }
    private IEnumerator AwaitFindPlayer() {
            yield return new WaitForSeconds(1);
            Start();
    }
}
