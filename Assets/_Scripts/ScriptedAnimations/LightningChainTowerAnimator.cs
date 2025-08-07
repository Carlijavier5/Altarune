using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LightningChainTowerAnimator : MonoBehaviour {
    [SerializeField] private Transform tower;

    public void FireAnim() {
        StartCoroutine(FireAnimAction());
    }

    private IEnumerator FireAnimAction() {
        tower.DOScale(new Vector3(1.1f, 0.5f, 1.1f), 0.1f).SetEase(Ease.OutQuart);
        yield return new WaitForSeconds(0.1f);
        tower.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetEase(Ease.OutBounce);
    }
}
