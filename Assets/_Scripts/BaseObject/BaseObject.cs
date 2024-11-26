using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The object that all interactable objects inherit from; <br/>
/// When object A interacts with object B you interact through this base object class; <br/>
/// </summary>
[DisallowMultipleComponent]
public abstract partial class BaseObject : MonoBehaviour {

    [SerializeField] protected Transform objectBody;
    public Transform ObjectBody => objectBody;

    public void DetachModules() {
        ObjectModule[] modules = GetComponentsInChildren<ObjectModule>(true);
        for (int i = 0; i < modules.Length; i++) Destroy(modules[i]);
    }

    public bool IsFaction(EntityFaction exceptionFaction) {
        Entity entity = this as Entity;
        return entity && exceptionFaction == entity.Faction;
    }

    public bool IsFactions(EntityFaction[] exceptionFactions) {
        return exceptionFactions.Any((faction) => IsFaction(faction));
    }

    #region || Material Swap Utilities ||

    private Renderer[] renderers;
    private readonly LinkedList<Material> materialStack = new();
    private readonly Dictionary<Renderer, Material[]> baseMaterialMap = new();

    public void UpdateRendererRefs(bool updateMaterials = true) {
        renderers = objectBody.GetComponentsInChildren<Renderer>(true);
        if (updateMaterials) {
            foreach (Renderer renderer in renderers) {
                baseMaterialMap[renderer] = renderer.sharedMaterials;
            }
        }
    } 

    public void ResetMaterials() {
        materialStack.Clear();
        try {
            foreach (KeyValuePair<Renderer, Material[]> kvp in baseMaterialMap) {
                kvp.Key.sharedMaterials = kvp.Value;
            }
        } catch {
            Debug.LogError("NRE: Invalid material reference cache");
        }
    }

    public void ApplyMaterial(Material material) {
        materialStack.Remove(material);
        materialStack.AddLast(material);
        SetMaterial(material);
    }

    public void UpdatePropertyBlock(System.Action<MaterialPropertyBlock> modAction) {
        MaterialPropertyBlock mpb = new();
        try {
            foreach (KeyValuePair<Renderer, Material[]> kvp in baseMaterialMap) {
                Renderer renderer = kvp.Key;
                renderer.GetPropertyBlock(mpb);
                modAction?.Invoke(mpb);
                renderer.SetPropertyBlock(mpb);
            }
        } catch {
            Debug.LogError("NRE: Invalid material reference cache");
        }
    }

    private void SetMaterial(Material material) {
        if (renderers == null) UpdateRendererRefs(true);
        try {
            foreach (Renderer renderer in renderers) {
                renderer.sharedMaterials = new Material[renderer.sharedMaterials.Length]
                                               .Select((mat) => material).ToArray();
            }
        } catch {
            Debug.LogError("NRE: Invalid material reference cache");
        }
    }

    public void RemoveMaterial(Material material) {
        if (materialStack.Count == 0) return;
        if (materialStack.Last.Value == material) {
            materialStack.RemoveLast();
            if (materialStack.Count == 0) ResetMaterials();
            else SetMaterial(materialStack.Last.Value);
        } else {
            LinkedListNode<Material> node = materialStack.FindLast(material);
            materialStack.Remove(node);
        }
    }

    #endregion
}