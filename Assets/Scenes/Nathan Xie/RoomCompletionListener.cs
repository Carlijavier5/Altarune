using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;

public class RoomCompletionListener : MonoBehaviour
{
    //Work with the assumption that additional enemies will not be summoned after the creation of the room or will not be necessary for completion
    [SerializeField] private List<Entity> currentEnemies; 

    void Start()
    {
        foreach(Entity i in currentEnemies){
            i.OnPerish += Enemy_OnPerish;
        }
    }

    private void Enemy_OnPerish(BaseObject baseObject){
        currentEnemies.Remove(baseObject as Entity);
        if (currentEnemies.Count == 0) {
            Debug.Log("Room is Empty. There are no enemies remaining");
        }
    }
}
