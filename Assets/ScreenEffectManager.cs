using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScreenEffectManager : MonoBehaviour
{
    private static ScreenEffectManager instance;
    public static ScreenEffectManager Instance => instance;

    [Header("Hit Effect")] [SerializeField]
    private Material hitMaterial;
    [SerializeField] private float hitRadius = 2.1f;
    [SerializeField] private float hitDelay = 0.0f;
    [SerializeField] private float hitDuration = 0.3f;
    
    // ------------------------ LOCAL MEMBERS -------------------------------
    private float elapsedTime = 0f;
    private bool effectRunning;
    // ----------------------------------------------------------------------
    
    void Awake() {
        //thank u mr. los
        if (instance) {
            Destroy(gameObject);
        } else instance = this;
        
        hitMaterial.SetFloat("_Radius", 10f);
    }

    public void HitEffect() {
        if (!effectRunning) {
            StartCoroutine(HitEffectAction());
        }
    }
    
    private IEnumerator HitEffectAction()
    {
        effectRunning = true;
        elapsedTime = 0f;

        hitMaterial.DOFloat(hitRadius, "_Radius", hitDelay);
        yield return new WaitForSeconds(hitDelay);
        hitMaterial.DOFloat(10f, "_Radius", hitDuration);
        effectRunning = false;
    }
}
