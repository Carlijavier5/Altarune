using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavageSpinEffector : MonoBehaviour {

    [SerializeField] private Collider attackCollider;

    public void ToggleDamage(bool on) {
        StopAllCoroutines();
        StartCoroutine(IClearContacts(on));
    }

    private IEnumerator IClearContacts(bool on) {
        attackCollider.enabled = false;
        yield return new WaitForEndOfFrame();
        attackCollider.enabled = on;
    }
}