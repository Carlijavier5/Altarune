using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SovereignStartTrigger : MonoBehaviour {

    [SerializeField] private SovereignGolem sovereignRef;
    [SerializeField] private Crystal[] crystals;
    private readonly HashSet<BaseObject> crystalSet = new();

    void Awake() {
        foreach (Crystal crystal in crystals) {
            crystalSet.Add(crystal);
            crystal.OnPerish += Crystal_OnPerish;
        }
    }

    private void Crystal_OnPerish(BaseObject crystal) {
        if (crystal) crystal.OnPerish -= Crystal_OnPerish;
        crystalSet.Remove(crystal);
        if (crystalSet.Count <= 0) {
            sovereignRef.BeginBossFight();
        }
    }

    public void InitTriggers() {
        foreach (Crystal crystal in crystals) {
            crystal.gameObject.SetActive(true);
        }
    }
}
