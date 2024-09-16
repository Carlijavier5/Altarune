using System;
using UnityEngine;
using static Shinobi;

public class Shinobi_AggroRadius : MonoBehaviour
{
    [SerializeField] private Shinobi shinobi;

    private void Awake()
    {
        gameObject.GetComponent<SphereCollider>().radius = shinobi.chaseDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player _) && shinobi.shouldChange)
        {
            shinobi.Aggro();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player _) && shinobi.stateMachine.State != new State_Follow() && shinobi.shouldChange)
        {
            shinobi.stateMachine.SetState(new State_Follow());
        }
    }
}
