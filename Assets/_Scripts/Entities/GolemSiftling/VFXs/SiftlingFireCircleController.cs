using UnityEngine;

public class SiftlingFireCircleController : MonoBehaviour
{
    private readonly int COLOR_PROP = Shader.PropertyToID("_Color");
    private readonly int TINT_COLOR_PROP = Shader.PropertyToID("_TintColor");

    [SerializeField] private ParticleSystem particleSystemRoot;
    [SerializeField] private ParticleSystemRenderer[] circleRenderers;
    [SerializeField] private ParticleSystemRenderer[] outerRenderers;
    [SerializeField] private AnimationCurve alphaCurve;
    [SerializeField] private float stopDuration;

    private enum State { On, Off }
    private State state;

    private Color[] circleBaseColors, outerBaseColors;
    private MaterialPropertyBlock mpb;

    private float alpha, spawnDuration;

    void Awake() {
        enabled = false;

        mpb = new();
        circleBaseColors = CacheBaseColors(circleRenderers, COLOR_PROP);
        outerBaseColors = CacheBaseColors(outerRenderers, TINT_COLOR_PROP);

        ApplyAlpha(circleRenderers, circleBaseColors, COLOR_PROP, alpha);
        ApplyAlpha(outerRenderers, outerBaseColors, TINT_COLOR_PROP, alpha);
    }
    
    public void Play(float duration) {
        spawnDuration = duration;

        state = State.On;
        particleSystemRoot.Play(true);
        enabled = true;
    }

    public void Lock() {
        enabled = false;
    }

    public void Stop() {
        state = State.Off;
        enabled = true;
    }

    void Update() {
        switch (state) {
            case State.On:
                alpha = Mathf.MoveTowards(alpha, 1, Time.deltaTime.SafeDivide(spawnDuration));
                ApplyAlpha(alphaCurve.Evaluate(alpha));
                break;
            case State.Off:
                alpha = Mathf.MoveTowards(alpha, 0, Time.deltaTime.SafeDivide(stopDuration));
                ApplyAlpha(alpha);

                if (alpha <= 0) {
                    particleSystemRoot.Stop(true);
                    enabled = false;
                }
                break;
        }
    }

    private Color[] CacheBaseColors(ParticleSystemRenderer[] renderers, int propertyID) {
        Color[] colors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++) {
            colors[i] = renderers[i].sharedMaterial.GetColor(propertyID);
        }
        return colors;
    }

    private void ApplyAlpha(float alpha) {
        ApplyAlpha(circleRenderers, circleBaseColors, COLOR_PROP, alpha);
        ApplyAlpha(outerRenderers, outerBaseColors, TINT_COLOR_PROP, alpha);
    }

    private void ApplyAlpha(ParticleSystemRenderer[] renderers, Color[] baseColors, int propertyID, float alpha) {
        for (int i = 0; i < renderers.Length; i++) {
            Color color = new(baseColors[i].r, baseColors[i].g, baseColors[i].b, alpha);
            renderers[i].GetPropertyBlock(mpb);
            mpb.SetColor(propertyID, color);
            renderers[i].SetPropertyBlock(mpb);
        }
    }
}