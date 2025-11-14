using System.Collections;
using UnityEngine;

public class GraphicFader : MonoBehaviour {

    protected const string COLOR_PARAM = "_Color";

    [SerializeField] private Renderer decal;
    [SerializeField] protected float fadeTime;

    void OnEnable() {
        Color color = decal.sharedMaterial.GetColor(COLOR_PARAM);

        MaterialPropertyBlock mpb = new();
        decal.GetPropertyBlock(mpb);

        color.a = 0;

        mpb.SetColor(COLOR_PARAM, color);
        decal.SetPropertyBlock(mpb);
    }

    public void DoFade(bool on) {
        StopAllCoroutines();
        StartCoroutine(IDoFade(on ? 1 : 0));
    }

    public void DoFade(float targetAlpha) {
        StopAllCoroutines();
        StartCoroutine(IDoFade(targetAlpha));
    }

    public IEnumerator IDoFade(float target) {
        MaterialPropertyBlock mpb = new();
        decal.GetPropertyBlock(mpb);

        Color color = mpb.GetColor(COLOR_PARAM);
        float alpha = color.a;
        while (Mathf.Abs(alpha - target) > 0) {
            alpha = Mathf.MoveTowards(alpha, target, fadeTime == 0 ? Mathf.Infinity : (Time.deltaTime / fadeTime));
            color.a = alpha;
            mpb.SetColor(COLOR_PARAM, color);
            decal.SetPropertyBlock(mpb);
            yield return null;
        }
    }
}