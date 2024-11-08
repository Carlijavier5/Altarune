using UnityEngine;

public enum EaseCurve { Fixed, Linear, InFixedOut, InLogarithmic, Logarithmic }

public class DefaultEaseCurves : ScriptableObject {
    [SerializeField] private AnimationCurve fullFixed, inFixedOut, linear,
                                            inLogarithmic, logarithmic;

    public AnimationCurve GetCurve(EaseCurve easeCurve) {
        return easeCurve switch {
            EaseCurve.InFixedOut => inFixedOut,
            EaseCurve.Linear => linear,
            EaseCurve.InLogarithmic => inLogarithmic,
            EaseCurve.Logarithmic => logarithmic,
            _ => fullFixed,
        };
    }
}