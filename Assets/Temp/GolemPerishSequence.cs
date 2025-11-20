using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemPerishSequence : MonoBehaviour
{
    private enum RendererType { Body, Crystal };

    private const string DISSOLVE_PARAM = "_Dissolve";
    private const string TEXTURE_PERCENT_PARAM = "_TexturePercent";

    [SerializeField] private Entity entity;
    [SerializeField] private Animator animator;
    [SerializeField] private Renderer[] bodyRenderers, crystalRenderers;
    [SerializeField] private ParticleSystem deathParticles;
    [System.Serializable] private class MaterialReplacement { public Material baseMaterial, replacementMaterial; }
    [SerializeField] private MaterialReplacement[] materialReplacements;
    [SerializeField] private Rigidbody[] crystalRigidbodies;
    [SerializeField] private Collider[] crystalColliders;
    [SerializeField] private float dissolveTime, textureMorphTime, crystalDropTime, crystalDissolveWait;

    private readonly Dictionary<Material, Material> materialReplacementMap = new();

    private class RBCache {
        public Vector3 position;
        public Quaternion rotation;
    }
    private readonly Dictionary<Rigidbody, RBCache> restitutionMap = new();

    private MaterialPropertyBlock mpb;

    void Awake() {
        foreach (MaterialReplacement matReplacement in materialReplacements) {
            materialReplacementMap[matReplacement.baseMaterial] = matReplacement.replacementMaterial;
        }
        foreach (Rigidbody rb in crystalRigidbodies) {
            restitutionMap[rb] = new() {
                position = rb.transform.position,
                rotation = rb.transform.rotation
            };
        }
    }

    public virtual void DoPerish() {        
        deathParticles.Play();
        DoOnRenderers(RendererType.Body, (renderer) => {
            ReplaceRendererMaterials(renderer);
        });

        mpb = new();
        bodyRenderers.First().GetPropertyBlock(mpb);

        StopAllCoroutines();
        StartCoroutine(IDoMaterialProperty(DISSOLVE_PARAM, dissolveTime, RendererType.Body));
        StartCoroutine(IDoMaterialProperty(TEXTURE_PERCENT_PARAM, textureMorphTime, RendererType.Body));
        StartCoroutine(IDoCrystalDrop());
    }

    private IEnumerator IDoMaterialProperty(string materialProperty, float time, RendererType rendererType) {
        
        float propertyLerp = mpb.GetFloat(materialProperty);

        while (propertyLerp < 1) {
            propertyLerp = Mathf.MoveTowards(propertyLerp, 1, Time.deltaTime.SafeDivide(time));
            mpb.SetFloat(materialProperty, propertyLerp);
            DoOnRenderers(rendererType, (mr) => mr.SetPropertyBlock(mpb));
            yield return null;
        }
    }

    private IEnumerator IDoCrystalDrop() {
        yield return new WaitForSeconds(crystalDropTime);
        ToggleCrystalPhysics(true);
        yield return new WaitForSeconds(crystalDissolveWait);
        DissolveCrystals();
    }

    private void DoOnRenderers(RendererType rendererType, System.Action<Renderer> callback) {
        Renderer[] renderers = rendererType switch { RendererType.Crystal => crystalRenderers,
                                                     _ => bodyRenderers };
        foreach (Renderer renderer in renderers) {
            callback.Invoke(renderer);
        }
    }

    private void ReplaceRendererMaterials(Renderer renderer) {
        if (entity.baseMaterialMap.TryGetValue(renderer, out Material[] sharedMaterials)) {
            Material[] mats = sharedMaterials;
            for (int i = 0; i < mats.Length; i++) {
                if (materialReplacementMap.TryGetValue(mats[i], out Material replacement)) {
                    mats[i] = replacement;
                }
            }
            renderer.sharedMaterials = mats;
        }
    }

    private void ToggleCrystalPhysics(bool on) {
        animator.enabled = !on;
        foreach (Rigidbody rb in crystalRigidbodies) {
            rb.isKinematic = !on;
            rb.transform.SetParent(null);
        }
        foreach (Collider coll in crystalColliders) {
            coll.enabled = on;
        }
    }

    private void DissolveCrystals() {
        crystalRenderers.First().GetPropertyBlock(mpb);
        DoOnRenderers(RendererType.Crystal, (renderer) => {
            ReplaceRendererMaterials(renderer);
        });
        StartCoroutine(IDoMaterialProperty(DISSOLVE_PARAM, dissolveTime, RendererType.Crystal));
    }

    private void Restitute() {

    }
}
