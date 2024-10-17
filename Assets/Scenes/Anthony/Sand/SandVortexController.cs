using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SandVortexController : MonoBehaviour
{
    #region Properties

    [SerializeField] new Renderer renderer;
    // Far camera
    [SerializeField] float minDepth = 5;
    // Near camera
    [SerializeField] float maxDepth = 12;

    [SerializeField] float cameraMinDist = 1;
    [SerializeField] float cameraMaxDist = 18;

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
    
    
    void Start()
    {
        
    }

    void Update()
    {
        // Camera distance
        Vector2 cameraXZ = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.z);
        Vector2 posXZ = new Vector2(transform.position.x, transform.position.z);
        float distanceFromCamera = Vector2.Distance(cameraXZ, posXZ);
        float newDepth = Mathf.Lerp(maxDepth, minDepth, Mathf.InverseLerp(cameraMinDist, cameraMaxDist, distanceFromCamera));

        //Debug.Log("Dist: " + distanceFromCamera);
        
        // Update vortex depth based on camera dist
        if (renderer)
        {
            renderer.GetPropertyBlock(Mpb, 0);
            
            mpb.SetFloat("_Depth", newDepth);
            
            renderer.SetPropertyBlock(Mpb, 0);
        }
    }
}
