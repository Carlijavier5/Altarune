using UnityEngine;

public static class GeneralUtils
{
    public static float SafeDivide(this float dividend, float divisor, float outputIfZero = Mathf.Infinity) {
        return divisor == 0 ? outputIfZero : (dividend / divisor);
    }

    public static void DeepIterate(this Transform root, System.Action<Transform> callback) {
        callback?.Invoke(root);
        foreach (Transform t in root) {
            t.DeepIterate(callback);
        }
    }
}