using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    private float damageTimer = 0.0f;
    public float waitTime = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(damageTimer>waitTime){
            Debug.Log("Damage Yan");
            damageTimer = 0.0f;
        }
    }
    private void OnTriggerStay(Collider collider){
        if (collider.gameObject.layer == 3){
            damageTimer += Time.deltaTime;
            
        }
    }
    private void OnTriggerExit(Collider collider){
        if(collider.gameObject.layer == 3){
            damageTimer = 0.0f;
        }
    }

}
