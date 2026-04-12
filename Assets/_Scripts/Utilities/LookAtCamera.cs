using UnityEngine;
using Unity.Cinemachine;

public class LookAtCamera : MonoBehaviour {

    private CinemachineBrain cameraBrain;
    private Camera OutputCamera => cameraBrain ? cameraBrain.OutputCamera
                                               : Camera.main;

    void Awake() {
        Camera.main.TryGetComponent(out cameraBrain);
    }

    void Update() => transform.rotation = OutputCamera.transform.rotation;
}
