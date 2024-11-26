using System.Collections.Generic;
using UnityEngine;

public class SovereignPhaseMaster<T> : MonoBehaviour
                             where T : SovereignPhaseConfiguration {

    [SerializeField] private T[] phaseConfigurations;

    private readonly Dictionary<SovereignPhase, T> phaseConfigMap = new();
    protected T activeConfig;

    protected virtual void Awake() {
        foreach (T config in phaseConfigurations) {
            phaseConfigMap[config.sovereignPhase] = config;
        }
    }

    public virtual void EnterPhase(SovereignPhase phase) {
        if (phaseConfigMap.ContainsKey(phase)) {
            activeConfig = phaseConfigMap[phase];
        } else {
            Debug.LogWarning($"No configuration for boss phase {phase}");
        }
    }
}