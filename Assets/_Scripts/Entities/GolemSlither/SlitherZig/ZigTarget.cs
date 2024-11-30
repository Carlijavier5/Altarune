using System.Collections;
using UnityEngine;

public class ZigTarget : ZigGraphic {

    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 startScale;
    private Vector3 targetScale;

    void Awake() {
        targetScale = transform.localScale;
    }

    public void GenerateAt(Vector3 position) {
        transform.position = position + positionOffset;
        transform.localScale = startScale;
        DoFade(true);
        StartCoroutine(IGenerate(position));
    }

    private IEnumerator IGenerate(Vector3 position) {
        float lerpVal, timer = 0;
        Vector3 generationPos = position + positionOffset;
        while (timer < fadeTime) {
            timer = Mathf.MoveTowards(timer, fadeTime, Time.deltaTime);
            lerpVal = timer / fadeTime;
            transform.position = Vector3.Lerp(generationPos, position, lerpVal);
            transform.localScale = Vector3.Lerp(startScale, targetScale, lerpVal);
            yield return null;
        }
    }
}