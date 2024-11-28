using UnityEngine;
using static GolemSlither;

public class GolemSlither_AggroRadius : MonoBehaviour
{
    [SerializeField] private GolemSlither golemSlither;

    private void OnTriggerEnter(Collider other) {
        if (golemSlither != null && golemSlither.shouldChange && other.TryGetComponent(out Player _)) {
            golemSlither.Aggro();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (golemSlither != null && golemSlither.shouldChange && other.TryGetComponent(out Player _)) {
            golemSlither.stateMachine.SetState(new State_Follow());
        }
    }
}
