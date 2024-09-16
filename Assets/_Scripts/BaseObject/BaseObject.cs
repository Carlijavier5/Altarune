using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The object that all interactable objects inherit from; <br/>
/// When object A interacts with object B you interact through this base object class; <br/>
/// </summary>
public abstract partial class BaseObject : MonoBehaviour {

    #region || Local Timescale ||

    protected float timeScale = 1;
    public virtual float TimeScale { get => timeScale; set => timeScale = value; }

    protected float DeltaTime => Time.deltaTime * timeScale;

    #endregion

    public bool Perished { get; private set; }

    /// <summary>
    /// Override to implement a death behavior for the object; <br/>
    /// </summary>
    public virtual void Perish() {
        Perished = true;
        OnPerish?.Invoke(this);
    }

    public void DetachModules() {
        ObjectModule[] modules = GetComponentsInChildren<ObjectModule>(true);
        for (int i = 0; i < modules.Length; i++) Destroy(modules[i]);
    }

    #region || Material Swap Utilities ||

    private Renderer[] renderers;
    private Dictionary<Renderer, Material[]> materialDict = new();

    public void UpdateRendererRefs(bool updateMaterials = true) {
        renderers = GetComponentsInChildren<Renderer>(true);
        if (updateMaterials) {
            foreach (Renderer renderer in renderers) {
                materialDict[renderer] = renderer.sharedMaterials;
            }
        }
    } 

    public void ResetMaterials() {
        try {
            foreach (KeyValuePair<Renderer, Material[]> kvp in materialDict) {
                kvp.Key.sharedMaterials = kvp.Value;
            }
        } catch {
            Debug.LogError("NRE: Invalid material reference cache");
        }
    }

    public void SetMaterial(Material material) {
        try {
            foreach (Renderer renderer in renderers) {
                renderer.sharedMaterials = new Material[renderer.sharedMaterials.Length]
                                               .Select((mat) => material).ToArray();
            }
        } catch {
            Debug.LogError("NRE: Invalid material reference cache");
        }
    }

    #endregion
}