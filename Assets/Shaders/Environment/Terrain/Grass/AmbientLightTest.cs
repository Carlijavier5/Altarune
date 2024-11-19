using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class AmbientLightTest : MonoBehaviour
{
    // Start is called before the first frame update
    private new Light light;

    [SerializeField] private float intensityVarianceStrength;
    [SerializeField] private float intensityVarianceAmplitude;
    
    [SerializeField] private float innerSpotSize;
    [SerializeField] private float outerSpotSize;
    void Start() {
        light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update() {
        light.intensity += Mathf.Sin(Time.time * intensityVarianceAmplitude) * intensityVarianceStrength;
        light.innerSpotAngle += Mathf.Sin(Time.time * intensityVarianceAmplitude) * innerSpotSize;
        light.spotAngle += Mathf.Sin(Time.time * intensityVarianceAmplitude) * outerSpotSize;
    }
}
