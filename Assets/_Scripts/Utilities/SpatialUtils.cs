using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpatialUtils {

    public static float WrapAngle360(float inputAngle) {
        return (inputAngle + 360) % 360;
    }

    public static float XZAngle360(this Vector3 input) {
        float angle = Vector3.SignedAngle(Vector3.right, input, Vector3.up);
        return 360 - (angle >= 0 ? angle : angle + 360); 
    }

    public static Quaternion ShortestPathTo(this Quaternion input, Quaternion other) {
        if (Quaternion.Dot(input, other) < 0) {
            return input * Quaternion.Inverse(Multiply(other, -1));
        } else return input * Quaternion.Inverse(other);
    }

    private static Quaternion Multiply(this Quaternion input, float scalar) {
        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }

    public static Vector2 RandomPointInRing(float minRadius, float maxRadius) {
        float theta = Random.Range(0.0f, 2.0f * Mathf.PI);
        float w = Random.Range(0.0f, 1.0f);
        float r = Mathf.Sqrt((1.0f - w) * minRadius * minRadius + w * maxRadius * maxRadius);
        return new Vector2(r * Mathf.Cos(theta), r * Mathf.Sin(theta));
    }
}
