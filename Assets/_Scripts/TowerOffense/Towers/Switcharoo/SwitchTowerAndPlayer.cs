using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTowerAndPlayer : MonoBehaviour
{
    //For the transformations
    [SerializeField] private Transform playerPosition, towerPosition;
    [SerializeField] private GameObject player, tower;
    private Vector3 playerOriginal, towerOriginal;

    //For attaching object to tower
    [SerializeField] private float radius;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask layerMask;

    RaycastHit enemiesHit;

    public void OnMouseUpAsButton()
    {
        //Attach enemy to tower
        CastSphere();
        Transform enemiesTransform = null;

        if (enemiesHit.transform != null)
        {
            enemiesTransform = enemiesHit.transform.parent;
            enemiesHit.transform.SetParent(tower.transform);
        }

        //Swap!
        playerOriginal = new Vector3(player.transform.position.x, 1, player.transform.position.z);
        player.SetActive(false);
        playerPosition.position = new Vector3(tower.transform.position.x, 0, tower.transform.position.z);
        towerPosition.position = playerOriginal;
        player.SetActive(true);

        if (enemiesHit.transform != null)
        {
            enemiesHit.transform.SetParent(enemiesTransform);
        }

        Invoke(nameof(destroyTower), 2);
    }

    private void destroyTower() {
        Destroy(tower);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position-transform.up*maxDistance,radius);
    }

    void CastSphere()
    {
        if (Physics.SphereCast(transform.position,radius,-transform.up,out enemiesHit,maxDistance,~layerMask))
        {
            if (enemiesHit.transform.TryGetComponent(out Golem Golem_Idle)) //W reddit
            {
                //enemiesHit.transform.SetParent(tower.transform);
                Debug.Log(enemiesHit.collider.gameObject);
            }
        }
    }


    /*
        public IEnumerator OnOpenButtonClick()
            {
            playerOriginal = playerPosition.position;
            player.SetActive(false);
            yield return new WaitForSeconds(0.01f);
            playerPosition.position = towerPosition.position;
            towerPosition.position = playerOriginal;
            yield return new WaitForSeconds(0.01f);
            player.SetActive(true);

            Debug.Log("it works?");
        }
    */


    //Ignore all this
    /*
    PlayerController playerController;
    [SerializeField] GameObject player;
    [SerializeField] GameObject tower;

    private void Start()
    {
        playerController = player.GetComponent<PlayerController>();
    }



*/
    /*
        //public Transform playerDestination;
        //public Transform towerDestionation;
        public GameObject Player;
        public GameObject Tower;

        public float speed = 1;

        private void OnMouseUpAsButton()
        {
            Tower.transform.position = Vector3.MoveTowards(Tower.transform.position, Player.transform.position, speed);
        }
    */

}