using UnityEngine;

public enum ElementType { Physical, Fire, Ice, Poison }

[System.Serializable]
public class HealthAttributes {

    [SerializeField] private AttributeCurves curves;

    public int health;
    [Range(0, 1)] public float defense;
    [Range(0, 1)] public float healModifier;

    public HealthAttributes(AttributeCurves curves) {
        this.curves = curves;
    }

    public int ComputeDamage(int amount) {
        return Mathf.FloorToInt(amount * curves.defenseCurve.Evaluate(defense));
    }

    public int ComputeHeal(int amount) {
        return Mathf.FloorToInt(amount * curves.healModCurve.Evaluate(healModifier));
    }
}

[System.Serializable]
public class AttributeCurves {
    public AnimationCurve defenseCurve, healModCurve;
    public AttributeCurves Clone() => (AttributeCurves) MemberwiseClone();
}