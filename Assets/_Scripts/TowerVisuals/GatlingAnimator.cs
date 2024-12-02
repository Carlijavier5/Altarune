using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.VFX;

public class GatlingAnimator : MonoBehaviour
{
    [Tooltip("Burst Fire Parameters")]
    [SerializeField] private Transform body;
    [SerializeField] private Transform muzzleFlashFX;
    [SerializeField] private Transform bulletFX;
    [SerializeField] private Transform groundFX;
    [SerializeField] private float windupDuration = 1f;
    private float fireDuration = -1f;
    [SerializeField] private int rounds = 16;
    [SerializeField] private float recoil = 0.3f;

    [Tooltip("Full-Auto Parameters")]
    [SerializeField] private bool debugMode;

    private Vector3 activePos;
    private IEnumerator activeAction;
    private Tween shake;
    void Start() {
        muzzleFlashFX.gameObject.SetActive(false);
        bulletFX.gameObject.SetActive(false);
        groundFX.gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (debugMode)
            if (Input.GetMouseButton(0)) {
                if (activeAction == null) {
                    activePos.y = body.position.y;
                    activeAction = FireAction();
                    StartCoroutine(activeAction);
                }
            } else if (Input.GetKey(KeyCode.E)) {
                FullAuto();
            }
    }

    public void SemiFire(Vector3 aimPos, float fireDuration) {
        if (this.fireDuration < 0) {
            shake = body.transform.DOShakePosition(fireDuration, recoil, 200, 90f, false, false).SetAutoKill(false);
        }
        float dist = (aimPos - transform.position).magnitude;
        bulletFX.GetComponent<VisualEffect>().SetFloat("Lifetime",  0f);
        activePos = aimPos;
        this.fireDuration = fireDuration;
        if (activeAction == null) {
            //activePos.y = body.position.y;
            activeAction = FireAction();
            StartCoroutine(activeAction);
        }
    }

    private IEnumerator FireAction() {
        body.DOLookAt(activePos, windupDuration, AxisConstraint.None, Vector3.up).SetEase(Ease.InOutElastic);
        StartCoroutine(FireFX());
        yield return new WaitForSecondsRealtime(fireDuration);
        activeAction = null;
        yield return null;
    }

    private IEnumerator FireFX() {
        yield return new WaitForSeconds(windupDuration / 2);
        muzzleFlashFX.gameObject.SetActive(true);
        bulletFX.gameObject.SetActive(true);
        groundFX.gameObject.SetActive(true);
        groundFX.position = activePos;
        shake.Restart();
        for (int i = 0; i < rounds * fireDuration; i++) {
            muzzleFlashFX.GetComponent<VisualEffect>().Play();
            bulletFX.GetComponent<VisualEffect>().Play();
            yield return null;
            yield return new WaitForSeconds(1f / rounds);
        }
        muzzleFlashFX.gameObject.SetActive(false);
        bulletFX.gameObject.SetActive(false);
        groundFX.gameObject.SetActive(false);
        yield return null;
    }
    
    private IEnumerator _activeFlashInstance;
    private void FullAuto()
    {
        if (!shake.IsPlaying()) {
            shake.Restart();
        }

        if (_activeFlashInstance == null) {
            _activeFlashInstance = ReplayVFXAfterDelay();
            StartCoroutine(_activeFlashInstance);
        }

        // Get direction to the enemy
        // Vector3 direction = aim.position - transform.position;
        // direction.y = 0;  // Optional: Ignore Y axis to keep rotation only on the horizontal plane
        //
        // // Get the target rotation to face the enemy
        // Quaternion targetRotation = Quaternion.LookRotation(direction);
        //
        // // Gradually rotate towards the target rotation using Lerp
        // transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
    
    private IEnumerator ReplayVFXAfterDelay() {
        yield return new WaitForSeconds(fireDuration / rounds / 2);
        muzzleFlashFX.GetComponent<VisualEffect>().Play();
        bulletFX.GetComponent<VisualEffect>().Play();
        _activeFlashInstance = null;
        yield return null;
    }
}
