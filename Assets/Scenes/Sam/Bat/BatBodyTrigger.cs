using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBodyTrigger : MonoBehaviour
{
    private BatBehavior _batBehavior;

    private bool _canAttack;
    void Start() {
        _batBehavior = GetComponentInParent<BatBehavior>();
        _canAttack = true;
        
    }

    void OnTriggerEnter(Collider other) {
        if (!_canAttack) return;
        if (other.gameObject.GetComponent<Player>() == null) return;

        _batBehavior.dealDamage();

    }


    public void setCanAttack(bool canAttack) {
        _canAttack = canAttack;
    }
}
