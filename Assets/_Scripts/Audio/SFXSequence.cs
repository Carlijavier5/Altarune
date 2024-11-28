using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXSequence : MonoBehaviour {

    [SerializeField] private SFXOneShot[] sfxSequence;
    private int sequenceIndex;

    public void Play() {
        SFXOneShot sfx = sfxSequence[sequenceIndex];
        sfx.Play();
        sequenceIndex = (sequenceIndex + 1) % sfxSequence.Length;
    }
}