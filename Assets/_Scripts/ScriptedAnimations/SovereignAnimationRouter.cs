using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SovereignAnimationRouter : MonoBehaviour {

    [SerializeField] private SovereignGolem golem;

    public void Animator_OnSlamLanding(LeftOrRight leftOrRight) {
        golem.Animator_OnSlamLanding(leftOrRight);
    }

    public void Animator_OnCollapsionLanding() {
        golem.Animator_OnCollapsionLanding();
    }
}