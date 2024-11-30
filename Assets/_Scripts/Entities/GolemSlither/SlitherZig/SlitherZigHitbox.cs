using System.Collections.Generic;
using UnityEngine;

public class SlitherZigHitbox : MonoBehaviour {

    [SerializeField] private ZigTrail zigTrail;
    [SerializeField] private ZigTarget zigTarget;

    public void Generate(Vector3 position, Quaternion rotation,
                         float zScale, Vector3 targetPosition) {
        zigTrail.Generate(position, rotation, zScale);
        zigTarget.GenerateAt(targetPosition);
    }

    public void DoDamage(int damageAmount) {
        zigTrail.DoDamage(damageAmount);
        Deactivate();
    }

    public void Deactivate() {
        zigTrail.DoFade(false);
        zigTarget.DoFade(false);
    }
}