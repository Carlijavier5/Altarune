using System.Collections.Generic;
using UnityEngine;

public class ArtificialBattery : Summon, IBattery {

    [SerializeField] private ManaConnection manaConnectionPrefab;
    [SerializeField] private GameObject areaIndicator;
    private readonly Dictionary<Summon, ManaConnection> linkedTowerMap = new();

    public ManaSource ManaSource => manaSource;
    public Vector3 Position => transform.position;
    public bool IsActive => active;

    public void LinkTower(Summon tower) {
        if (!linkedTowerMap.ContainsKey(tower)) {
            ManaConnection mcInstance = Instantiate(manaConnectionPrefab);
            mcInstance.Init(transform, tower.transform);
            linkedTowerMap[tower] = mcInstance;
        }
    }

    public override void Collapse() {
        foreach (KeyValuePair<Summon, ManaConnection> kvp in linkedTowerMap) {
            ManaConnection mc = kvp.Value;
            mc.Disconnect(true);
            Summon tower = kvp.Key;
            tower.Collapse();
        } base.Collapse();
    }

    public void ToggleArea(bool on) {
        areaIndicator.SetActive(on);
    }
}