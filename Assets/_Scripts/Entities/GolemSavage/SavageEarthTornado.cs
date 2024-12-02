using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavageEarthTornado : MonoBehaviour {

    [SerializeField] private LoopingSystemController loopController;
    [SerializeField] private DynamicTransformScaler tScaler;
    [SerializeField] private float scaleMeshDelay;

    public void Activate() {
        StopAllCoroutines();
        loopController.Enable();
        StartCoroutine(IDelayMeshScale());
    }

    public void Deactivate() {
        StopAllCoroutines();
        loopController.Disable();
        tScaler.DoScale(false);
    }

    private IEnumerator IDelayMeshScale() {
        yield return new WaitForSeconds(scaleMeshDelay);
        
    }
}