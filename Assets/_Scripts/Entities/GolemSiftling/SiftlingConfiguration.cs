using UnityEngine;

[System.Serializable]
public class SiftlingConfiguration {

    [Header("General")]
    public SiftlingType type;
    public int health;
    public SiftlingTornado tornado;
    [Header("Roam Properties")]
    public float animationSpeed;
    public float roamSpeed;
    public Vector2 distanceRange;
    public Vector2 waitRange;
}