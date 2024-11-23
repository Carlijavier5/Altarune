using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TeleportFXTest : MonoBehaviour
{
    public Renderer targetRenderer;
    public string propertyName = "_Teleport_Threshold";
    public float startValue = 0f;
    public float endValue = 1f;
    public float duration = 1f; // Duration of the lerp
    public float delay = 0f;

    public VisualEffect effect;

    private MaterialPropertyBlock _propertyBlock;
    private float _lerpTimer = 0f;
    private bool _isLerping = false;

    void Start()
    {
        // Initialize MaterialPropertyBlock
        _propertyBlock = new MaterialPropertyBlock();
        targetRenderer.GetPropertyBlock(_propertyBlock);

        // Set the starting value
        _propertyBlock.SetFloat(propertyName, startValue);
        targetRenderer.SetPropertyBlock(_propertyBlock);
        _propertyBlock.SetFloat(propertyName, startValue);
    }

    void Update()
    {
        // Check for spacebar press
        if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(Run());
        }

        // Perform the lerp if active
        if (_isLerping)
        {
            _lerpTimer += Time.deltaTime;

            float t = Mathf.Clamp01(_lerpTimer / duration); // Normalize time
            float lerpedValue = Mathf.Lerp(startValue, endValue, t);

            // Apply the lerped value to the property block
            _propertyBlock.SetFloat(propertyName, lerpedValue);
            targetRenderer.SetPropertyBlock(_propertyBlock);

            // Stop lerping when the duration is reached
            if (t >= 1f)
            {
                _isLerping = false;
            }
        }
    }

    IEnumerator Run() {
        yield return new WaitForSeconds(delay);
        _isLerping = true;
        _lerpTimer = 0f; // Reset the time
        effect.transform.position = transform.position;
        effect.Play();
    }
}
