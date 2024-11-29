using System.Collections;
using UnityEngine;

public abstract class ZigGraphic : MonoBehaviour {

    protected const string COLOR_PARAM = "_Color";

    [SerializeField] private Renderer decal;
    [SerializeField] protected float fadeTime;
    private float timer;

    void OnEnable() {
        Color color = decal.sharedMaterial.GetColor(COLOR_PARAM);
        color.a = 0;
        MaterialPropertyBlock mpb = new();
        decal.GetPropertyBlock(mpb);
        mpb.SetColor(COLOR_PARAM, color);
        decal.SetPropertyBlock(mpb);
    }

    public void DoFade(bool on) {
        StartCoroutine(IDoFade(on));
    }

    public IEnumerator IDoFade(bool on) {
        float lerpVal, target = on ? fadeTime : 0;
        Color color = decal.sharedMaterial.GetColor(COLOR_PARAM);

        MaterialPropertyBlock mpb = new();
        decal.GetPropertyBlock(mpb);

        while (Mathf.Abs(target - timer) > 0) {
            timer = Mathf.MoveTowards(timer, target, Time.deltaTime);
            lerpVal = timer / fadeTime;
            color.a = Mathf.Lerp(0, 1, lerpVal);
            mpb.SetColor(COLOR_PARAM, color);
            decal.SetPropertyBlock(mpb);
            yield return null;
        }
    }
}