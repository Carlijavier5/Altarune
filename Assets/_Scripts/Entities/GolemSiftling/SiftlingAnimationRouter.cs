using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiftlingAnimationRouter : MonoBehaviour
{
    public event System.Action OnBodyAirborne;

    /// <summary>
    /// Called by the 'Jump' and 'RaiseLeg' animations to begin
    /// rotation in the FaceTarget state;
    /// </summary>
    public void Animator_OnBodyAirborne() => OnBodyAirborne?.Invoke();
}
