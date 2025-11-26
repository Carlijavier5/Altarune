using System.Collections;
using UnityEngine;

public class SFXOneShotRandom : MonoBehaviour {
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private Vector2 pitchVariation;
    [SerializeField] private float volumeMult = 1;

    public void Play() {
        source.volume = GM.AudioManager.SFXVolume * volumeMult;
        source.pitch = 1 + Random.Range(pitchVariation.x, pitchVariation.y);
        int index = Random.Range(0, clips.Length);
        source.PlayOneShot(clips[index]);
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