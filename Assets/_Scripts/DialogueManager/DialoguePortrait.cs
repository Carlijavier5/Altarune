using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePortrait : MonoBehaviour {

    [SerializeField] private Image image;
    [SerializeField] private Color hideTint;
    [SerializeField] private float toggleTime,
                                   fadeTime;
    private float toggleTimer, fadeTimer;
    private Coroutine toggleCoroutine, fadeCoroutine;

    void Awake() {
        Color color = image.color;
        color.a = 0;
        image.color = color;
    }

    public void AssignSprite(Sprite sprite) {
        image.sprite = sprite;
        image.SetNativeSize();
    }

    public void Toggle(bool on) {
        if (toggleCoroutine != null) StopCoroutine(toggleCoroutine);
        toggleCoroutine = StartCoroutine(IToggle(on));
    }

    public void Materialize(bool on) {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(IMaterialize(on));
    }

    private IEnumerator IToggle(bool on) {
        float lerpVal, target = on ? toggleTime : 0;
        while (Mathf.Abs(target - toggleTimer) > 0) {
            toggleTimer = Mathf.MoveTowards(toggleTimer, target, Time.unscaledDeltaTime);
            lerpVal = toggleTimer / toggleTime;
            Color color = Color.Lerp(hideTint, Color.white, lerpVal);
            color.a = image.color.a;
            image.color = color;
            yield return null;
        }
    }

    private IEnumerator IMaterialize(bool on) {
        float lerpVal, alpha,
              target = on ? fadeTime : 0;
        while (Mathf.Abs(target - fadeTimer) > 0) {
            fadeTimer = Mathf.MoveTowards(fadeTimer, target, Time.unscaledDeltaTime);
            lerpVal = fadeTimer / fadeTime;
            alpha = Mathf.Lerp(0, 1, lerpVal);
            image.color = new(image.color.r, image.color.g,
                              image.color.b, alpha);
            yield return null;
        }
    }
}