using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemPerishSequence : MonoBehaviour
{
    private const string DISSOLVE_PARAM = "_Dissolve";
    private const string TEXTURE_PERCENT_PARAM = "_TexturePercent";
    
    [SerializeField] private Renderer[] meshRenderers;
    [SerializeField] private ParticleSystem deathParticles;
    [System.Serializable] private class MaterialReplacement { public Material baseMaterial, replacementMaterial; }
    [SerializeField] private MaterialReplacement[] materialReplacements;
    [SerializeField] private Rigidbody[] crystalRBs;
    [SerializeField] private float dissolveTime, textureMorphTime, weaponDropTime;

    private readonly Dictionary<Material, Material> materialReplacementMap = new();
    private MaterialPropertyBlock mpb;

    void Awake() {
        foreach (MaterialReplacement matReplacement in materialReplacements) {
            materialReplacementMap[matReplacement.baseMaterial] = matReplacement.replacementMaterial;
        }
    }

    public void DoPerish() {
        DoOnRenderers((mr) => {
            Material[] mats = mr.sharedMaterials;
            for (int i = 0; i < mats.Length; i++) {
                if (materialReplacementMap.TryGetValue(mr.sharedMaterials[i], out Material replacement)) {
                    mats[i] = replacement;
                }
            }
            mr.sharedMaterials = mats;
        });

        mpb = new();
        meshRenderers.First().GetPropertyBlock(mpb);

        StopAllCoroutines();
        StartCoroutine(IDoMaterialProperty(DISSOLVE_PARAM, dissolveTime));
        StartCoroutine(IDoMaterialProperty(TEXTURE_PERCENT_PARAM, textureMorphTime));
    }

    private IEnumerator IDoMaterialProperty(string materialProperty, float time) {
        
        float propertyLerp = mpb.GetFloat(materialProperty);

        while (propertyLerp < 1) {
            propertyLerp = Mathf.MoveTowards(propertyLerp, 1, Time.deltaTime.SafeDivide(time));
            mpb.SetFloat(materialProperty, propertyLerp);
            DoOnRenderers((mr) => mr.SetPropertyBlock(mpb));
            yield return null;
        }
    }

    private void DoOnRenderers(System.Action<Renderer> callback) {
        foreach (Renderer renderer in meshRenderers) {
            callback.Invoke(renderer);
        }
    }
}
