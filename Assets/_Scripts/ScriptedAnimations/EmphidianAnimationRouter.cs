using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmphidianAnimationRouter : MonoBehaviour {

    [SerializeField] private Emphidian emphidian;

    public void Animator_ReleaseProjectile() {
        emphidian.Animator_ReleaseProjectile();
    }
}