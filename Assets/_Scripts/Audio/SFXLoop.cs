using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXLoop : MonoBehaviour {

    [SerializeField] private AudioSource[] sources;
    [SerializeField] private AudioClip clip;
    [SerializeField] private Vector2 pitchVariation;
    [SerializeField] private float repeatTime;

    private int sourceIndex;

    public void Play() {
        StartCoroutine(IPlay());
    }

    public void Stop(float duration = 0.1f) {
        StartCoroutine(IStop(duration));
    }

    private IEnumerator IPlay() {
        AudioSource source = sources[sourceIndex];
        sourceIndex = (sourceIndex + 1) % sources.Length;
        source.volume = GM.AudioManager.MusicVolume;
        source.pitch = Random.Range(pitchVariation.x, pitchVariation.y);
        source.PlayOneShot(clip);
        yield return new WaitForSeconds(repeatTime);
    }

    private IEnumerator IStop(float duration) {
        float lerpVal, timer = 0,
              currVolume = GM.AudioManager.SFXVolume;
        while (timer < duration) {
            timer += Time.unscaledDeltaTime;
            lerpVal = timer / duration;
            foreach (AudioSource source in sources) {
                source.volume = Mathf.Lerp(currVolume, 0, lerpVal);
            } yield return null;
        }
    }
}