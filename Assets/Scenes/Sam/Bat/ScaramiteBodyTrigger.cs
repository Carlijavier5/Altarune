using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaramiteBodyTrigger : MonoBehaviour
{
    private Scaramite _batBehavior;

    private bool _canAttack;
    void Start() {
        _batBehavior = GetComponentInParent<Scaramite>();
        _canAttack = true;
        
    }

    //Detects if the player has touched the bat and should take damage
    void OnTriggerEnter(Collider other) {
        if (!_canAttack) return;
        if (other.TryGetComponent(out Player player)) {
            _batBehavior.DealDamage(player);
        }
    }


    //Sets the bat's attack status
    public void SetCanAttack(bool canAttack) {
        _canAttack = canAttack;
    }
}
