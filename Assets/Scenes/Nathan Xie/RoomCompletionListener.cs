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
    public HashSet<Entity> currentEnemies = new(); 

    void Start()
    {
        Entity[] objects = UnityEngine.Object.FindObjectsOfType<Entity>();
        foreach(Entity i in objects){
            if (i.Faction == EntityFaction.Hostile) {
                currentEnemies.Add(i);
                i.OnPerish += Enemy_OnPerish;
            }
        }
    }

    private void Enemy_OnPerish(BaseObject baseObject){
        currentEnemies.Remove(baseObject as Entity);
        if (currentEnemies.Count == 0) {
            Debug.Log("Room is Empty. There are no enemies remaining");
        }
    }
}
