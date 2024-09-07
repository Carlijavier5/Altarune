using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[ExecuteAlways]
public class TimeMagicCircleController : MonoBehaviour
{
    #region Properties

    [Header("Components")]
    [SerializeField] Renderer renderer;

    [Space] [Header("Animation Settings")]
    [SerializeField] float radius = 7f;
    Vector3 mainRotationAxis = Vector3.up;
    [SerializeField] float mainRotationRate = 15;
    [SerializeField] float secondaryRotationRate = 8;
    [SerializeField] [Range(0f, 1f)] float secondaryScale = 0.85f;

    [SerializeField] [ColorUsage(true, true)]
    Color positiveColor;
    [SerializeField] [ColorUsage(true, true)]
    Color negativeColor;
    [SerializeField] AnimationCurve modeChangeCurve;
    [SerializeField] AnimationCurve transitionScaleCurve;
    [SerializeField] float transitionDuration = 0.75f;
    
    [SerializeField] ParticleSystem psPositive;
    [SerializeField] ParticleSystem psNegative;

    [Space] [Header("Debug")]
    [ReadOnly] Color currentColor;
    [ReadOnly] float currentDirectionalMultiplier = -1f;
    
    TimeTowerState state;
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
    
    #endregion
    
    #region Mono
    void Start()
    {
        
    }

    void Update()
    {
        // Update main rotation
        transform.RotateAround(transform.position, mainRotationAxis, mainRotationRate * Time.deltaTime * currentDirectionalMultiplier);
    }

    // Updates effect properties on any value change
    void OnValidate()
    {
        OnValidateUpdateAnimationParams();
    }

    #endregion
    
    #region Functions

    // Only during OnValidate
    void OnValidateUpdateAnimationParams()
    {
        if (!renderer)
        {
            renderer = GetComponent<Renderer>();
        }
        
        // Radius
        UpdateCircleRadius();
        
        // Colors and secondary rotation
        Color color = (state == TimeTowerState.Forward) ? positiveColor : negativeColor;
        if (renderer)
        {
            renderer.GetPropertyBlock(Mpb, 0);
            
            mpb.SetFloat("_SecondaryScale", secondaryScale);
            mpb.SetFloat("_SecondarySpinRate", secondaryRotationRate);
            mpb.SetColor("_Color", color);
            
            renderer.SetPropertyBlock(Mpb, 0);
        }
    }

    void UpdateCircleCurrentColor()
    {
        if (renderer)
        {
            renderer.GetPropertyBlock(Mpb, 0);
            
            mpb.SetColor("_Color", currentColor);
            
            renderer.SetPropertyBlock(Mpb, 0);
        }
    }

    void UpdateCircleRadius()
    {
        transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);
    }
    
    public void SwitchDirections()
    {
        switch (state)
        {
            case TimeTowerState.Forward:
                state = TimeTowerState.Backward;
                break;
            case TimeTowerState.Backward:
                state = TimeTowerState.Forward;
                break;
        }

        StartCoroutine(OnStateTransition());
    }

    public IEnumerator OnStateTransition()
    {
        yield return null;

        float timeElapsed = 0f;

        float startDirection = state == TimeTowerState.Forward ? 1f : -1f;
        float endDirection = startDirection * -1f;

        Color startColor = state == TimeTowerState.Forward ? negativeColor : positiveColor;
        Color endColor = state == TimeTowerState.Forward ? positiveColor : negativeColor;

        float radiusBeforeAnim = radius;
        
        // Particle systems
        switch (state)
        {
            case TimeTowerState.Forward:
                psPositive.Play();
                psNegative.Stop();
                break;
            case TimeTowerState.Backward:
                psPositive.Stop();
                psNegative.Play();
                break;
        }
        
        while (timeElapsed < transitionDuration)
        {
            timeElapsed += Time.deltaTime;

            float timeAlpha = timeElapsed / transitionDuration;
            float animAlpha = modeChangeCurve.Evaluate(timeAlpha);
            float directionalMultiplier = Mathf.Lerp(startDirection, endDirection, animAlpha);
            currentDirectionalMultiplier = directionalMultiplier;
            currentColor = Color.Lerp(startColor, endColor, animAlpha);
            UpdateCircleCurrentColor();

            float scaleAlpha = transitionScaleCurve.Evaluate(timeAlpha);
            radius = radiusBeforeAnim * scaleAlpha;
            UpdateCircleRadius();
            
            yield return null;
        }
    }
    
    #endregion
    
}

public enum TimeTowerState
{
    Forward,
    Backward,
}