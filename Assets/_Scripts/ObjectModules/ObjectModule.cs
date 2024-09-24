using UnityEngine;

[RequireComponent(typeof(BaseObject))]
public abstract class ObjectModule : MonoBehaviour {

    [SerializeField] protected BaseObject baseObject;

    protected virtual void Reset() {
        TryGetComponent(out baseObject);
    }
}

public class EventResponse { public bool received = false; }