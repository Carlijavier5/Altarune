using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiftlingAnimationRouter : MonoBehaviour {

    [SerializeField] private GolemSiftling siftling;

    public void Animator_OnAscensionRisen() {
        siftling.Animator_OnAscensionRisen();
    }

    public void Animator_OnDescent() {
        siftling.Animator_OnDescent();
    }
}