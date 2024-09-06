using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shinobi_SweepRadius : MonoBehaviour
{
    [NonSerialized] public bool ShouldSweep = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player _))
        {
            ShouldSweep = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player _))
        {
            ShouldSweep = false;
        }
    }
}
