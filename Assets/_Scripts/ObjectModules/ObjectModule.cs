using UnityEngine;

[RequireComponent(typeof(BaseObject))]
public abstract class ObjectModule : MonoBehaviour {

    [SerializeField] protected BaseObject baseObject;

    #if UNITY_EDITOR
    protected virtual void Reset() {
        TryGetComponent(out baseObject);
        ObjectModule[] modules = GetComponents<ObjectModule>();
        foreach (ObjectModule module in modules) module.EDITOR_ONLY_AttachModule();
    }

    public virtual void EDITOR_ONLY_AttachModule() { }
    #endif
}

public class EventResponse { public bool received = false; }
public class EventResponse<T> : EventResponse {
    public T objectReference;
}