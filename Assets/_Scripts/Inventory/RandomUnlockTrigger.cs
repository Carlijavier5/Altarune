using UnityEngine;

public class RandomUnlockTrigger : MonoBehaviour {

    [SerializeField] private DynamicTransformScaler scaler;
    [SerializeField] private Collider contactCollider;
    
    private TowerUnlockType unlockType;
    private TowerData towerData;

    void OnEnable() {
        if (towerData != null) {
            switch (unlockType) {
                case TowerUnlockType.Tower:
                    if (GM.InventoryManager.HasTower(towerData)) {
                        scaler.DoScale(false);
                        contactCollider.enabled = false;
                    } break;
                case TowerUnlockType.Inversion:
                    if (GM.InventoryManager.HasInversion(towerData)) {
                        scaler.DoScale(false);
                        contactCollider.enabled = false;
                    } break;
            }
        }
    }

    public void DoUnlockRune() {
        if (towerData != null) return;
        if (TryFetchUnlock(out unlockType, out towerData)) {
            scaler.DoScale(true);
            contactCollider.enabled = true;
        }
    }

    private bool TryFetchUnlock(out TowerUnlockType unlockType,
                                out TowerData towerData) {
        TowerUnlockType type = (TowerUnlockType) Random.Range(0, 1);
        if (type == TowerUnlockType.Tower) {
            if (GM.InventoryManager.TryGetRandomTowerUnlock(out towerData)) {
                unlockType = TowerUnlockType.Tower;
                return true;
            } else if (GM.InventoryManager.TryGetRandomInversionUnlock(out towerData)) {
                unlockType = TowerUnlockType.Inversion;
                return true;
            }
        } else if (type == TowerUnlockType.Inversion) {
            if (GM.InventoryManager.TryGetRandomInversionUnlock(out towerData)) {
                unlockType = TowerUnlockType.Inversion;
                return true;
            } else if (GM.InventoryManager.TryGetRandomTowerUnlock(out towerData)) {
                unlockType = TowerUnlockType.Tower;
                return true;
            }
        }

        unlockType = 0;
        towerData = null;
        return false;
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player _)) {
            switch (unlockType) {
                case TowerUnlockType.Tower:
                    GM.InventoryManager.UnlockTower(towerData);
                    break;
                case TowerUnlockType.Inversion:
                    GM.InventoryManager.UnlockInversion(towerData);
                    break;
            }
        }    
    }
}