using System.Collections;
using UnityEngine;

public class SFXManagedLoop : MonoBehaviour {

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;
    [SerializeField] private bool playOnAwake;

    private void Awake() {
        source.clip = clip;
        if (playOnAwake) Play();
    }

    public void Play() {
        StopAllCoroutines();
        source.volume = GM.AudioManager.SFXVolume;
        source.Play();
    }

    public void Stop(float duration = 0.1f) {
        StopAllCoroutines();
        StartCoroutine(IStop(duration));
    }

    private IEnumerator IStop(float duration) {
        float lerpVal, timer = 0,
              currVolume = GM.AudioManager.SFXVolume;
        while (timer < duration) {
            timer += Time.unscaledDeltaTime;
            lerpVal = timer / duration;
            source.volume = Mathf.Lerp(currVolume, 0, lerpVal);
            yield return null;
        }
    }
}