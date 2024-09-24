using UnityEngine;
using static Shinobi;

public class Shinobi_AggroRadius : MonoBehaviour
{
    [SerializeField] private Shinobi shinobi;

    private void OnTriggerEnter(Collider other)
    {
        if (shinobi != null && shinobi.shouldChange && other.TryGetComponent(out Player _))
        {
            shinobi.Aggro();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (shinobi != null && shinobi.shouldChange && other.TryGetComponent(out Player _))
        {
            shinobi.stateMachine.SetState(new State_Follow());
        }
    }
}
