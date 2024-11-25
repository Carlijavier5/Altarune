using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerStance { Standard, Alternate }

public class InversionModule : MonoBehaviour {

    [SerializeField] private Summon summon;
    [SerializeField] private InversionLoader inversionLoader;

    void Awake() {
        //inversionLoader.OnInversionEnd += InversionLoader_OnInversionEnd;
    }

    private void Summon_RequestInversionAccess(EventResponse response) {
        response.received = true;
    }

    //private void Summon_OnInversionRequest() => summon.TerminateStance();

    private void Summon_OnStanceTerminated() => inversionLoader.InvertStance();

    //private void InversionLoader_OnInversionEnd() => summon.Reinit();

    #if UNITY_EDITOR
    void Reset() {
        TryGetComponent(out summon);
    }
    #endif
}
