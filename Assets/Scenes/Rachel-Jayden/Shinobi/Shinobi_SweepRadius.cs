using System;
using UnityEngine;

public class Shinobi_SweepRadius : MonoBehaviour
{
    [NonSerialized] public bool shouldSweep = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player _))
        {
            shouldSweep = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player _))
        {
            shouldSweep = false;
        }
    }
}
