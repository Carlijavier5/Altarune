using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabCircle : MonoBehaviour
{
    [SerializeField] private float waitForSeconds;
    private float timer = 0f;

    void OnTriggerStay(Collider other) {
        timer += Time.deltaTime;

        if (timer >= waitForSeconds) {
            SceneLoader.Instance.Load(SceneLoader.Scene.M2_Game);
        }
    }

    void OnTriggerExit(Collider other) {
        timer = 0f;
    }

    private IEnumerator TeleportPlayer() {
        yield return new WaitForSeconds(waitForSeconds);

    }
}
