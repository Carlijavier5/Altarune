using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamplePullSource : MonoBehaviour {

    private HashSet<BaseObject> baseObjects;

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out BaseObject baseObject)) {
            baseObjects.Add(baseObject);
        }
    }

    private void FixedUpdate() {
        foreach (BaseObject baseObject in baseObjects) {
            baseObject.TryPush(transform.position - baseObject.transform.position, 4);
        }
    }
}
