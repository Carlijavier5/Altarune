using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class RailgunAnimator : MonoBehaviour {
    [Header("Railgun Model Properties)")]
    [SerializeField] private Transform gatlingBody;
    [SerializeField] private Transform gatlingPivot;
    [SerializeField] private Transform railgunModel;
    [SerializeField] private Transform railgunBody;
    [SerializeField] private Transform railgunPivot;
    [SerializeField] private Transform railgunGems;
    [SerializeField] private bool debugMode;
    [SerializeField] private VisualEffect railEffect;
    [SerializeField] private VisualEffect gatlingEffect;

    private bool _railgunActive = false;

    private void Awake() {
        railgunPivot.transform.localScale = new Vector3(0f, 1f, 1f);
        railgunGems.transform.localScale = new Vector3(1f, 0f, 0f);
        railgunModel.gameObject.SetActive(false);
        railEffect.Stop();
        gatlingEffect.Stop();
    }

    private void Update() {
        if (debugMode) {
            if (Input.GetKeyDown(KeyCode.R)) {
                if (!_railgunActive) StartCoroutine(RailgunActivate());
                else StartCoroutine(RailgunDeactivate());
            }
        }
    }

    private IEnumerator RailgunActivate() {
        railEffect.Stop();
        gatlingBody.gameObject.SetActive(false);
        railgunBody.rotation = gatlingBody.rotation;
        railgunModel.position = gatlingBody.position;
        railgunModel.gameObject.SetActive(true);
        
        //Gatling Retract
        gatlingPivot.DOScaleX(0f, 0.3f).SetEase(Ease.InBack);
        yield return new WaitForSeconds(0.2f);
        //Railgun Extend
        railgunPivot.DOScaleX(1f, 0.3f).SetEase(Ease.OutBack);
        railEffect.Reinit();
        yield return new WaitForSeconds(0.1f);
        railgunGems.DOScale(Vector3.one, 0.3f);
        _railgunActive = true;
        yield return null;
    }
    
    private IEnumerator RailgunDeactivate() {
        railgunGems.DOScale(new Vector3(1f, 0f, 0f), 0.3f);
        yield return new WaitForSeconds(0.1f);
        railgunPivot.DOScaleX(0f, 0.3f).SetEase(Ease.InBack);
        yield return new WaitForSeconds(0.2f);
        gatlingPivot.DOScaleX(1f, 0.3f).SetEase(Ease.OutBack);
        gatlingBody.rotation = railgunBody.rotation;
        gatlingBody.position = railgunModel.position;
        gatlingBody.gameObject.SetActive(true);
        gatlingEffect.Reinit();
        yield return new WaitForSeconds(0.2f);
        railgunModel.gameObject.SetActive(false);
        _railgunActive = false;
        yield return null;
    }
}
