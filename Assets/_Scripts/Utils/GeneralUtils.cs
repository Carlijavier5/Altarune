using UnityEngine;

public static class GeneralUtils
{
    public static float SafeDivide(this float dividend, float divisor, float outputIfZero = Mathf.Infinity) {
        return divisor == 0 ? outputIfZero : (dividend / divisor);
    }

    public static Vector3 SafeDivide(this Vector3 dividend, float divisor, float divideByIfZero = 1f) {
        return divisor == 0 ? dividend / (divideByIfZero) : (dividend / divisor);
    }

    public static void DeepIterate(this Transform root, System.Action<Transform> callback) {
        callback?.Invoke(root);
        foreach (Transform t in root) {
            t.DeepIterate(callback);
        }
    }

    public static bool Contains(this LayerMask layerMask, int layer) {
        return (layerMask & (1 << layer)) != 0;
    }
}