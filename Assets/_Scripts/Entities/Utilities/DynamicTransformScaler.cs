using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicTransformScaler : MonoBehaviour {

    [SerializeField] private ScaleGroup[] scaleGroups;
    [SerializeField] private float duration;
    private float timer;

    public void DoScale(bool on) {
        StopAllCoroutines();
        StartCoroutine(IDoScale(on));
    }

    private IEnumerator IDoScale(bool on) {
        foreach (ScaleGroup group in scaleGroups) {
            group.transform.gameObject.SetActive(true);
        }

        float lerpVal, target = on ? duration : 0;

        Vector3 scale;
        while (Mathf.Abs(timer - target) > Mathf.Epsilon) {
            timer = Mathf.MoveTowards(timer, target, Time.deltaTime);
            lerpVal = timer / duration;
            foreach (ScaleGroup group in scaleGroups) {
                scale = Vector3.Lerp(Vector3.zero, group.targetScale, lerpVal);
                group.transform.localScale = scale;
            }
            yield return null;
        }

        if (!on) {
            foreach (ScaleGroup group in scaleGroups) {
                group.transform.gameObject.SetActive(false);
            }
        }
    }
}

[System.Serializable]
public class ScaleGroup {
    public Transform transform;
    public Vector3 targetScale;
}