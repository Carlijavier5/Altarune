using UnityEngine;

[System.Serializable]
public class CCAttributes {

    protected readonly DefaultCrowdControlCurves ccCurves;
    [Range(0, 1)] public float ccResistance;
    [Range(0, 1)] public float stunResistance;
    [Range(0, 1)] public float slowResistance;
}