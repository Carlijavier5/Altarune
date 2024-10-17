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
    [SerializeField] private float fireDuration;
    [SerializeField] private int rounds = 16;
    [SerializeField] private float recoil = 0.3f;
    [SerializeField] private int vibrato = 40;
    [SerializeField] private Transform aim;

    [Tooltip("Full-Auto Parameters")] [SerializeField]
    private float rotationSpeed = 2.0f;
    [SerializeField] private bool debugMode;

    private Vector3 activePos;
    private IEnumerator activeAction;
    private Tween shake;
    void Start() {
        shake = body.transform.DOShakePosition(fireDuration / 2, recoil, vibrato, 90f, false, false).SetAutoKill(false);
    }
    
    void Update()
    {
        if (debugMode)
            if (Input.GetMouseButton(0)) {
                if (activeAction == null) {
                    activePos = aim.position;
                    activePos.y = body.position.y;
                    activeAction = FireAction();
                    StartCoroutine(activeAction);
                }
            } else if (Input.GetKey(KeyCode.E)) {
                FullAuto();
            }
    }

    public void SemiFire(Vector3 aimPos) {
        activePos = aimPos;
        if (activeAction == null) {
            activePos = aim.position;
            activePos.y = body.position.y;
            activeAction = FireAction();
            StartCoroutine(activeAction);
        }
    }

    private IEnumerator FireAction() {
        body.DOLookAt(activePos, fireDuration, AxisConstraint.None, Vector3.up).SetEase(Ease.InOutElastic);
        StartCoroutine(FireFX());
        yield return new WaitForSecondsRealtime(fireDuration);
        activeAction = null;
        yield return null;
    }

    private IEnumerator FireFX() {
        yield return new WaitForSeconds(fireDuration / 2);
        shake.Restart();
        for (int i = 0; i < rounds; i++) {
            muzzleFlashFX.GetComponent<VisualEffect>().Play();
            bulletFX.GetComponent<VisualEffect>().Play();
            yield return new WaitForSeconds(fireDuration / rounds / 2);
        }

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
        Vector3 direction = aim.position - transform.position;
        direction.y = 0;  // Optional: Ignore Y axis to keep rotation only on the horizontal plane

        // Get the target rotation to face the enemy
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Gradually rotate towards the target rotation using Lerp
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
    
    private IEnumerator ReplayVFXAfterDelay() {
        yield return new WaitForSeconds(fireDuration / rounds / 2);
        muzzleFlashFX.GetComponent<VisualEffect>().Play();
        bulletFX.GetComponent<VisualEffect>().Play();
        _activeFlashInstance = null;
        yield return null;
    }
}
