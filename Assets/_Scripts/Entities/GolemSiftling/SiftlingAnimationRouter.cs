using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiftlingAnimationRouter : MonoBehaviour
{
    public event System.Action OnBodyAirborne;

    [SerializeField] private ParticleSystem landingDust;

    /// <summary>
    /// Called by the 'Jump' and 'RaiseLeg' animations to begin
    /// rotation in the FaceTarget state;
    /// </summary>
    public void Animator_OnBodyAirborne() => OnBodyAirborne?.Invoke();

    /// <summary>
    /// Called by the 'Jump' animation on landing to spawn dust;
    /// </summary>
    public void Animator_OnBodyLanding() {
        landingDust.Play();
    }
}
