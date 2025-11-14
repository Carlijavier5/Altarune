using System.Collections;
using UnityEngine;

public class TwoColoredGraphicFader : MonoBehaviour {

    protected const string COLOR_PARAM = "_Color";
    protected const string COLOR_2_PARAM = "_Color_2";

    [SerializeField] private Renderer decal;
    [SerializeField] protected float fadeTime;

    void OnEnable() {
        Color color1 = decal.sharedMaterial.GetColor(COLOR_PARAM);
        Color color2 = decal.sharedMaterial.GetColor(COLOR_2_PARAM);

        MaterialPropertyBlock mpb = new();
        decal.GetPropertyBlock(mpb);

        color1.a = 0;
        color2.a = 0;

        mpb.SetVector(COLOR_PARAM, color1);
        mpb.SetVector(COLOR_2_PARAM, color2);
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

        Color color1 = mpb.GetColor(COLOR_PARAM);
        Color color2 = mpb.GetColor(COLOR_2_PARAM);

        float alpha1 = color1.a;
        float alpha2 = color2.a;

        while (Mathf.Abs(alpha1 - target) > 0
                || Mathf.Abs(alpha2 - target) > 0) {

            alpha1 = Mathf.MoveTowards(alpha1, target, fadeTime == 0 ? Mathf.Infinity : (Time.deltaTime / fadeTime));
            alpha2 = Mathf.MoveTowards(alpha2, target, fadeTime == 0 ? Mathf.Infinity : (Time.deltaTime / fadeTime));

            color1.a = alpha1;
            color2.a = alpha2;

            mpb.SetVector(COLOR_PARAM, color1);
            mpb.SetVector(COLOR_2_PARAM, color2);

            decal.SetPropertyBlock(mpb);
            yield return null;
        }
    }
}