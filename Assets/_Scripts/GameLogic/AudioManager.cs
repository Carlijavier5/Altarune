using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [SerializeField] private float masterVolume, musicVolume, soundVolume;
    [SerializeField] private AudioSource musicSource;
    
    public float MusicVolume => masterVolume * musicVolume;
    public float SFXVolume => masterVolume * soundVolume;

    public void PlayMusic(AudioClip clip) {
        musicSource.volume = MusicVolume;
        if (musicSource.clip != clip) {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void FadeMusic(float duration) {
        StopAllCoroutines();
        StartCoroutine(IFadeMusic(duration));
    }

    private IEnumerator IFadeMusic(float duration) {
        float lerpVal, timer = 0,
              currVolume = MusicVolume;
        while (timer < duration) {
            timer += Time.unscaledDeltaTime;
            lerpVal = timer / duration;
            musicSource.volume = Mathf.Lerp(currVolume, 0, lerpVal);
            yield return null;
        }
    }
}