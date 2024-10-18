using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCompletionListener : MonoBehaviour {

    public event System.Action OnRoomCleared;

    [SerializeField] private Entity[] currentEnemies;
    private readonly HashSet<Entity> enemySet = new();

    public bool Complete { get; private set; }

    void Awake() {
        foreach (Entity entity in currentEnemies) {
            if (enemySet.Add(entity)) {
                entity.OnPerish += Entity_OnPerish; ;
            }
        }
    }

    private void Entity_OnPerish(BaseObject baseObject){
        if (Complete) return;
        enemySet.Remove(baseObject as Entity);
        if (enemySet.Count == 0) CompleteRoom();
    }

    public void CompleteRoom() {
        OnRoomCleared?.Invoke();
        Complete = true;
    }
}
