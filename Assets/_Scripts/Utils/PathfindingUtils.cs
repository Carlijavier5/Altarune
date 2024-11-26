using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathfindingUtils {

    /// <summary>
    /// Find a clear roaming point around a source;
    /// </summary>
    /// <param name="source"> Origin of the search cast; </param>
    /// <param name="distance"> Distance of the cast; </param>
    /// <param name="angleRange"> Range of direction angles to pick; </param>
    /// <param name="clearPoint"> Output clear point, if one is found; </param>
    /// <returns> Whether a clear roaming point was found; </returns>
    public static bool FindBiasedRoamingPoint(Vector3 source, float distance, Vector2 angleRange, out Vector3 clearPoint) {
        float dirAngle = Random.Range(angleRange.x, angleRange.y);
        dirAngle = SpatialUtils.WrapAngle360(dirAngle);
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

    public static bool FindBiasedRoamingPoint(Vector3 source, float distance, Vector2 angleRange,
                                              int attempts, out Vector3 clearPoint) {
        int i = 0;
        while (i < attempts) {
            if (FindBiasedRoamingPoint(source, distance, angleRange, out clearPoint)) {
                return true;
            }
            i++;
        }
        clearPoint = Vector3.zero;
        return false;
    }

    public static bool FindRandomRoamingPoint(Vector3 source, float distance, int attempts, out Vector3 clearPoint) {
        return FindBiasedRoamingPoint(source, distance, new Vector2(0, 360), attempts, out clearPoint);
    }

    public static bool FindRandomRoamingPoint(Vector3 source, float distance, out Vector3 clearPoint) {
        return FindBiasedRoamingPoint(source, distance, new Vector2(0, 360), out clearPoint);
    }

    private static bool AngleCast(Vector3 source, float distance, float angle) {
        Vector3 dir = AngleToXZDirection(angle);
        return !Physics.Raycast(source, dir, distance, LayerUtils.EnvironmentLayerMask);
    }

    private static Vector3 AngleToXZDirection(float angle) {
        angle = SpatialUtils.WrapAngle360(angle);
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0,
                           Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
    }
}
