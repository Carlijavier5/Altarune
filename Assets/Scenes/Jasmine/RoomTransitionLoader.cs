using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Singleton: SceneFader for dungeon room transitions
/// 
/// is attatched to a canvas for fading the screen in and out
/// </summary>
public class RoomTransitionLoader : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    private float fadeTime = 1;
    private Image img;
    public static RoomTransitionLoader Instance { get; private set; }

    void Awake() {
        if (Instance != null && Instance != this) { Destroy(this); }
        else { Instance = this; }

        img = GetComponentInChildren<Image>();
    }

    public void FadeIn() {
        StartCoroutine(i_FadeIn());
    }

    public void FadeOut(bool immediateFade = false) {
        if (immediateFade) {
            img.color = new Color(0f, 0f, 0f, 1);
        } else {
            StartCoroutine(i_FadeOut());
        }
    }

    // ---------------

    private IEnumerator i_FadeIn() {
        float t = fadeTime;    // time

        while (t > 0) {
            t -= Time.unscaledDeltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return 0;         // wait a frame and then continue
        }
    }

    private IEnumerator i_FadeOut() {
        float t = 0;    // time

        while (t < fadeTime) {
            t += Time.unscaledDeltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return 0;         // wait a frame and then continue
        }
    }
}
