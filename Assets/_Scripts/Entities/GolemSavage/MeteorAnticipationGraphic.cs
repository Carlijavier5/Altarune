using System.Collections;
using UnityEngine;

public class MeteorAnticipationGraphic : MonoBehaviour {

    protected const string COLOR_PARAM = "_Color";
    protected const string COLOR_2_PARAM = "_Color_2";

    [SerializeField] private Renderer decal;
    [SerializeField] protected float fadeTime;

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
        MaterialPropertyBlock mpb = new();
        decal.GetPropertyBlock(mpb);
        Color color1 = mpb.GetColor(COLOR_PARAM);
        Color color2 = mpb.GetColor(COLOR_2_PARAM);

        float target = on ? 1 : 0;
        while (Mathf.Abs(color1.a - target) > 0
                || Mathf.Abs(color2.a - target) > 0) {
            color1.a = Mathf.MoveTowards(color1.a, target, fadeTime == 0 ? Mathf.Infinity : (Time.deltaTime / fadeTime));
            color2.a = Mathf.MoveTowards(color2.a, target, fadeTime == 0 ? Mathf.Infinity : (Time.deltaTime / fadeTime));
            mpb.SetColor(COLOR_PARAM, color1);
            mpb.SetColor(COLOR_2_PARAM, color2);
            decal.SetPropertyBlock(mpb);
            yield return null;
        }
    }
}