using System.Collections.Generic;
using UnityEngine;

public class RoomCompletionListener : MonoBehaviour {

    public event System.Action OnRoomCleared;
    public event System.Action<BaseObject> OnEntityPerish;

    [SerializeField] private Entity[] currentEnemies;
    private readonly HashSet<Entity> enemySet = new();

    public bool Complete { get; private set; }

    public void Init(RoomTag roomTag) {
        Dictionary<string, PerishInfo> perishMap = GM.RunManager.GetPerishMap(roomTag);
        foreach (Entity entity in currentEnemies) {
            if (perishMap.ContainsKey(entity.gameObject.name)) {
                entity.Perish(true);
            } else if (enemySet.Add(entity)) {
                entity.OnPerish += Entity_OnPerish;
            }
        }
    }

    private void Entity_OnPerish(BaseObject baseObject){
        if (Complete) return;
        enemySet.Remove(baseObject as Entity);
        if (enemySet.Count == 0
            && !Complete) CompleteRoom();
        OnEntityPerish?.Invoke(baseObject);
    }

    public void CompleteRoom() {
        OnRoomCleared?.Invoke();
        Complete = true;
    }
}
