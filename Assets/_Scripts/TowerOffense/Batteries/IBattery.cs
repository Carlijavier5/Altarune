using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattery {

    public ManaSource ManaSource { get; }
    public Vector3 Position { get; }
    public bool IsActive { get; }

    public void LinkTower(Summon tower);
    public void ToggleArea(bool on);

    public MonoBehaviour MonoScript => this as MonoBehaviour;
}
