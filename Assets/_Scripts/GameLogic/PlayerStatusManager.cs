using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusManager : MonoBehaviour
{
    public int Health { get; private set; }
    public bool HealthIsAtMax { get; private set; } = true;

    public void CommitHealth(int health) {
        if (health > 0) {
            Health = health;
            HealthIsAtMax = false;
        }
    }

    public void ResetHealthStatus() => HealthIsAtMax = true;
}
