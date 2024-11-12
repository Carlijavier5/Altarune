using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathfindingUtils {

    public static bool FindRandomRoamingPoint(Vector3 source, float distance, int attempts, out Vector3 clearPoint) {
        int i = 0;
        while (i < attempts) {
            if (FindRandomRoamingPoint(source, distance, out clearPoint)) {
                return true;
            } i++;
        }
        clearPoint = Vector3.zero;
        return false;
    }

    public static bool FindRandomRoamingPoint(Vector3 source, float distance, out Vector3 clearPoint) {
        float dirAngle = Random.Range(0, 360);
        Vector3 roamDir = AngleCast(source, distance, dirAngle) ? AngleToXZDirection(dirAngle)              /// Front
                        : AngleCast(source, distance, dirAngle + 180) ? AngleToXZDirection(dirAngle + 180)  /// Back
                        : AngleCast(source, distance, dirAngle + 90) ? AngleToXZDirection(dirAngle + 90)    /// Right
                        : AngleCast(source, distance, dirAngle - 90) ? AngleToXZDirection(dirAngle - 90)    /// Left
                        : AngleCast(source, distance, dirAngle + 45) ? AngleToXZDirection(dirAngle + 45)    /// Front Right
                        : AngleCast(source, distance, dirAngle - 45) ? AngleToXZDirection(dirAngle - 45)    /// Front Left
                        : AngleCast(source, distance, dirAngle + 135) ? AngleToXZDirection(dirAngle + 135)  /// Back Right
                        : AngleCast(source, distance, dirAngle - 135) ? AngleToXZDirection(dirAngle - 135)  /// Back Left
                        : Vector3.zero;                                                                     /// Unsuccessful

        clearPoint = roamDir * distance + source;
        return roamDir.magnitude > 0;
    }

    private static bool AngleCast(Vector3 source, float distance, float angle) {
        Vector3 dir = AngleToXZDirection(angle);
        return !Physics.Raycast(source, dir, distance, LayerUtils.EnvironmentLayerMask);
    }

    private static Vector3 AngleToXZDirection(float angle) {
        return new(Mathf.Cos(angle), 0, Mathf.Sin(angle));
    }
}
