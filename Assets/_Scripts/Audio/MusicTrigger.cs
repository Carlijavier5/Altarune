using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour {

    [SerializeField] private AudioClip clip;

    public void Play(float delay) {
        if (delay <= 0) {
            GM.AudioManager.PlayMusic(clip);
        } else {
            StartCoroutine(IPlayMusic(delay));
        }
    }

    private IEnumerator IPlayMusic(float delay) {
        yield return new WaitForSeconds(delay);
        GM.AudioManager.PlayMusic(clip);
    }
}