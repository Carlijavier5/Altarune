using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialReplacer : MonoBehaviour {

    [SerializeField] private Material material;
    
    public void ReplaceMaterialsInRenderers() {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers) {
            renderer.sharedMaterials = new[] { material };
        }
    }
}