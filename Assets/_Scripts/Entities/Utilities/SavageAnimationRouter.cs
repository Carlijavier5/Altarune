using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavageAnimationRouter : MonoBehaviour {

    [SerializeField] private GolemSavage savage;

    public void Animator_OnSpinEnd() {
        savage.Animator_OnSpinEnd();
    }

    public void Animator_OnSlamHit() {
        savage.Animator_OnSlamHit();
    }

    public void Animator_OnMeteorPunch() {
        savage.Animator_OnMeteorPunch();
    }
}
