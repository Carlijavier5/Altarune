using System.Collections;
using UnityEngine;

public class SFXOneShot : MonoBehaviour {
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;
    [SerializeField] private Vector2 pitchVariation;

    public AudioClip Clip => clip;

    public void Play() {
        source.volume = GM.AudioManager.SFXVolume;
        source.pitch = 1 + Random.Range(pitchVariation.x, pitchVariation.y);
        source.PlayOneShot(clip);
    }

    public void Stop(float duration = 0.1f) {
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