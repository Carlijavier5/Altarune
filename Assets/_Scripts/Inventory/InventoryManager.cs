using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerUnlockType { Tower = 0, Inversion = 1 }

public class InventoryManager : MonoBehaviour {

    [SerializeField] private TowerData[] runeSet;
    [SerializeField] private TowerData[] startingCoreRunes;
    [SerializeField] private TowerData[] startingInversions;
    private readonly Dictionary<TowerData, bool> towerUnlockMap = new();
    private readonly Dictionary<TowerData, bool> inversionUnlockMap = new();

    public readonly TowerData[] selectedSlots = new TowerData[3];

    void Awake() {
        foreach (TowerData data in runeSet) {
            towerUnlockMap[data] = false;
            inversionUnlockMap[data] = false;
        }

        foreach (TowerData data in startingCoreRunes) {
            towerUnlockMap[data] = true;
        }

        foreach (TowerData data in startingInversions) {
            inversionUnlockMap[data] = true;
        }
    }

    public TowerData[] AvailableTowers {
        get {
            List<TowerData> availableList = new();
            foreach (KeyValuePair<TowerData, bool> kvp in towerUnlockMap) {
                if (kvp.Value) availableList.Add(kvp.Key);
            }
            return availableList.ToArray();
        }
    }

    private TowerData[] UnavailableTowers {
        get {
            List<TowerData> unavailableList = new();
            foreach (KeyValuePair<TowerData, bool> kvp in towerUnlockMap) {
                if (!kvp.Value) unavailableList.Add(kvp.Key);
            }
            return unavailableList.ToArray();
        }
    }

    private TowerData[] UnavailableInversions {
        get {
            List<TowerData> unavailableList = new();
            foreach (KeyValuePair<TowerData, bool> kvp in inversionUnlockMap) {
                if (towerUnlockMap[kvp.Key] && !kvp.Value) {
                    unavailableList.Add(kvp.Key);
                }
            }
            return unavailableList.ToArray();
        }
    }

    public void UnlockTower(TowerData towerData) {
        towerUnlockMap[towerData] = true;
    }

    public void UnlockInversion(TowerData towerData) {
        towerUnlockMap[towerData] = true;
    }

    public bool TryGetRandomTowerUnlock(out TowerData towerData) {
        TowerData[] unavailableSet = UnavailableTowers;
        if (unavailableSet.Length == 0) {
            towerData = null;
            return false;
        } else {
            int index = Random.Range(0, unavailableSet.Length);
            towerData = unavailableSet[index];
            return true;
        }
    }

    public bool TryGetRandomInversionUnlock(out TowerData towerData) {
        TowerData[] unavailableSet = UnavailableInversions;
        if (unavailableSet.Length == 0) {
            towerData = null;
            return false;
        } else {
            int index = Random.Range(0, unavailableSet.Length);
            towerData = unavailableSet[index];
            return true;
        }
    }

    public bool HasTower(TowerData towerData) => towerUnlockMap[towerData];
    public bool HasInversion(TowerData towerData) => inversionUnlockMap[towerData];
}