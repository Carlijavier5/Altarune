using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SovereignSpawnMaster : SovereignPhaseMaster<SpawnerProperties> {

    public event System.Action OnSpawnPerish;

    [SerializeField] private SpawnPlatform[] platforms;

    private readonly HashSet<SpawnPlatform> activePlatforms = new();
    private float respawnTimer;
    private bool isRespawnPending;

    protected override void Awake() {
        base.Awake();
        foreach (SpawnPlatform crystalPlatform in platforms) {
            crystalPlatform.OnCrystalShatter += SpawnPlatform_OnSpawnPerish;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            EnterPhase(SovereignPhase.Macro1);
        }
        if (Input.GetKeyDown(KeyCode.N)) {
            EnterPhase(SovereignPhase.Macro2);
        }
        if (Input.GetKeyDown(KeyCode.M)) {
            EnterPhase(SovereignPhase.Macro3);
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            EnterPhase(SovereignPhase.Macro4);
        }

        if (isRespawnPending) {
            respawnTimer -= Time.deltaTime;
            if (respawnTimer <= 0) {
                SpawnEntity();
                isRespawnPending = false;
            }
        }
    }

    public override void EnterPhase(SovereignPhase phase) {
        base.EnterPhase(phase);
        StartSpawnTimer();
    }

    private void SpawnEntity() {
        List<SpawnPlatform> validPlatforms = platforms
                                             .Where((cp) => !cp.HasCrystal).ToList();
        int spawnAmount = activeConfig.maxSpawns - activePlatforms.Count;
        for (int i = 0; i < spawnAmount; i++) {
            int selectedPlatform = Random.Range(0, validPlatforms.Count);
            validPlatforms[i].SpawnEntity(activeConfig.spawnPrefab);
            activePlatforms.Add(validPlatforms[i]);
            validPlatforms.RemoveAt(i);
            if (validPlatforms.Count == 0) return;
        }
    }

    private void SpawnPlatform_OnSpawnPerish(SpawnPlatform platform) {
        activePlatforms.Remove(platform);
        if (respawnTimer <= 0) StartSpawnTimer();
        OnSpawnPerish?.Invoke();
    }

    private void StartSpawnTimer() {
        isRespawnPending = true;
        respawnTimer = Random.Range(activeConfig.spawnWaitRange.x,
                                    activeConfig.spawnWaitRange.y);
    }
}

[System.Serializable]
public class SpawnerProperties : SovereignPhaseConfiguration {
    public Entity spawnPrefab;
    public int maxSpawns;
    public Vector2 spawnWaitRange;
}