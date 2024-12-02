using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ManaShieldController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private new Renderer renderer;
    [SerializeField] private new ParticleSystem particleSystem;
    
    MaterialPropertyBlock mpb;
    public MaterialPropertyBlock Mpb
    {
        get
        {
            if (mpb == null)
                mpb = new MaterialPropertyBlock();
            return mpb;
        }
    }

    [Header("Animation Settings")]
    [SerializeField] float spawnDuration = 0.5f;
    [SerializeField] AnimationCurve spawnCurve;
    [SerializeField] float hitDuration = 0.2f;
    [SerializeField] AnimationCurve hitCurve;
    [SerializeField] float breakFadeOutDuration = 0.3f;
    [SerializeField] AnimationCurve fadeOutCurve;

    [Header("Debug")]
    [SerializeField] [Range(0f, 1f)] float debugSpawnAlpha = 0f;
    [SerializeField] [Range(0f, 1f)] float debugHitAlpha = 0f;
    [SerializeField] [Range(0f, 1f)] float debugBreakAlpha = 0f;

    public event System.Action onSpawn;
    public event System.Action onBreak;

    void OnValidate() {
        UpdateAnimationParams();
    }

    void UpdateAnimationParams() {
        UpdateSpawnAlpha(debugSpawnAlpha);
        UpdateHitAlpha(debugHitAlpha);
        UpdateBreakAlpha(debugBreakAlpha);
    }

    void UpdateSpawnAlpha(float alpha) {
        if (!renderer)
        {
            renderer = GetComponent<Renderer>();
        }
        
        // Colors and secondary rotation
        if (renderer)
        {
            renderer.GetPropertyBlock(Mpb, 0);
            
            mpb.SetFloat("_Spawn", alpha);
            
            renderer.SetPropertyBlock(Mpb, 0);
        }
    }
    
    void UpdateHitAlpha(float alpha) {
        if (!renderer)
        {
            renderer = GetComponent<Renderer>();
        }
        
        // Colors and secondary rotation
        if (renderer)
        {
            renderer.GetPropertyBlock(Mpb, 0);
            
            mpb.SetFloat("_Hit", alpha);
            
            renderer.SetPropertyBlock(Mpb, 0);
        }
    }
    
    void UpdateBreakAlpha(float alpha) {
        if (!renderer)
        {
            renderer = GetComponent<Renderer>();
        }
        
        // Colors and secondary rotation
        if (renderer)
        {
            renderer.GetPropertyBlock(Mpb, 0);
            
            mpb.SetFloat("_Break", alpha);
            
            renderer.SetPropertyBlock(Mpb, 0);
        }
    }
    
    public void ResetMaterialParams() {
        // Colors and secondary rotation
        if (renderer)
        {
            renderer.GetPropertyBlock(Mpb, 0);
            
            mpb.SetFloat("_Spawn", 0);
            mpb.SetFloat("_Hit", 0);
            mpb.SetFloat("_Break", 0);
            
            renderer.SetPropertyBlock(Mpb, 0);
        }
        
        particleSystem.Stop();
    }
    
    public void ShieldSpawn() {
        ResetMaterialParams();
        StartCoroutine(OnShieldSpawn());
    }
    
    public void SimulateHit() {
        StartCoroutine(OnSimulateHit());
    }
    
    public void ShieldBreak() {
        StartCoroutine(OnShieldBreak());
    }
    
    #region Coroutines
    IEnumerator OnShieldSpawn() {
        float t = 0f;
        
        while (t < hitDuration) {
            t += Time.deltaTime;

            float alpha = spawnCurve.Evaluate(t / spawnDuration);
            UpdateSpawnAlpha(alpha);
            yield return null;
        }
        
        particleSystem.Play();
        onSpawn?.Invoke();
    }
    
    IEnumerator OnSimulateHit() {
        float t = 0f;
        
        while (t < hitDuration) {
            t += Time.deltaTime;

            float alpha = hitCurve.Evaluate(t / hitDuration);
            UpdateHitAlpha(alpha);
            yield return null;
        }
    }
    
    IEnumerator OnShieldBreak() {
        float t = 0f;
        
        while (t < breakFadeOutDuration) {
            t += Time.deltaTime;

            float alpha = fadeOutCurve.Evaluate(t / breakFadeOutDuration);
            UpdateBreakAlpha(alpha);
            yield return null;
        }
        
        particleSystem.Stop();  
        onBreak?.Invoke();
    }
    #endregion
}
