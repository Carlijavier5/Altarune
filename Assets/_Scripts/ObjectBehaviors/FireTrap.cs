using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    private float damageTimer = 0.0f;
    [SerializeField]
    private float waitTime = 1.0f;
    private HashSet<GameObject> entitiesInTrap = new HashSet<GameObject>();
    // Update is called once per frame
    void Update()
    {
        if (entitiesInTrap.Count == 0) {
            damageTimer += Time.deltaTime; // Increment damageTimer if any entity is within FireTrap collider
        }
        else {
            damageTimer = 0.0f; // Reset damageTimer if there are no entitites
        }
        if (damageTimer >= waitTime){
            foreach (GameObject entity in entitiesInTrap) {
                Debug.Log("Do Damage"); // Damage all entitites within collider when damageTimer reaches threshold
            }
        } 
    }
    private void OnTriggerEnter(Collider other) {
        entitiesInTrap.Add(other.gameObject); // Add entity to list that enters collision box
    }
    private void OnTriggerExit(Collider other) {
        entitiesInTrap.Remove(other.gameObject); // Remove entity from list that leaves collision box
    }
}
