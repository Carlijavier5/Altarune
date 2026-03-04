using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatAudioController : MonoBehaviour
{
    [SerializeField] private GolemBat attachedBat;
    [SerializeField] private SFXOneShotRandom idleSFXSource, stunSFXSource,
                                              wallBonkSFXSource, perishSFXSource;
    [SerializeField] private Vector2 idleBarkTimeRange;
    [SerializeField] private float ccCD;
    private float canPlayCCTime;

    void Awake() {
        attachedBat.OnStunSet += AttachedBat_OnStunSet;
        StartCoroutine(IPlayIdle());
    }

    private void AttachedBat_OnStunSet(bool isStunned) {
        StopAllCoroutines();
        if (!isStunned) {
            StartCoroutine(IPlayIdle());
        } else if (Time.time > canPlayCCTime) {
            canPlayCCTime = Time.time + ccCD;
            stunSFXSource.Play();
        }
    }

    public void PlayWallBonk() {
        if (Time.time > canPlayCCTime) {
            canPlayCCTime = Time.time + ccCD;
            wallBonkSFXSource.Play();
        }
    }

    public void PlayPerish() {
        attachedBat.OnStunSet -= AttachedBat_OnStunSet;
        StopAllCoroutines();
        perishSFXSource.Play();
    }

    private IEnumerator IPlayIdle() {
        while (true) {
            float waitTime = Random.Range(idleBarkTimeRange.x, idleBarkTimeRange.y);
            yield return new WaitForSeconds(waitTime);
            idleSFXSource.Play();
        }
    }
}
