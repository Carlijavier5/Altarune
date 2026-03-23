using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    public static bool FindBiasedRoamingPointSphereCast(Vector3 source, float radius, float distance, Vector2 angleRange, out Vector3 clearPoint) {
        float dirAngle = Random.Range(angleRange.x, angleRange.y);
        dirAngle = SpatialUtils.WrapAngle360(dirAngle);
        Vector3 roamDir = AngleCastSphere(source, radius, distance, dirAngle) ? AngleToXZDirection(dirAngle)              /// Front
                        : AngleCastSphere(source, radius, distance, dirAngle + 180) ? AngleToXZDirection(dirAngle + 180)  /// Back
                        : AngleCastSphere(source, radius, distance, dirAngle + 90) ? AngleToXZDirection(dirAngle + 90)    /// Right
                        : AngleCastSphere(source, radius, distance, dirAngle - 90) ? AngleToXZDirection(dirAngle - 90)    /// Left
                        : AngleCastSphere(source, radius, distance, dirAngle + 45) ? AngleToXZDirection(dirAngle + 45)    /// Front Right
                        : AngleCastSphere(source, radius, distance, dirAngle - 45) ? AngleToXZDirection(dirAngle - 45)    /// Front Left
                        : AngleCastSphere(source, radius, distance, dirAngle + 135) ? AngleToXZDirection(dirAngle + 135)  /// Back Right
                        : AngleCastSphere(source, radius, distance, dirAngle - 135) ? AngleToXZDirection(dirAngle - 135)  /// Back Left
                        : Vector3.zero;                                                                                   /// Unsuccessful

        clearPoint = roamDir * distance + source;
        return roamDir.magnitude > 0;
    }

    public static bool FindBiasedRoamingPointSphereCast(Vector3 source, float radius, float distance, Vector2 angleRange,
                                                        int attempts, out Vector3 clearPoint) {
        int i = 0;
        while (i < attempts) {
            if (FindBiasedRoamingPointSphereCast(source, radius, distance, angleRange, out clearPoint)) {
                return true;
            }
            i++;
        }
        clearPoint = Vector3.zero;
        return false;
    }

    public static bool FindRandomRoamingPointSphereCast(Vector3 source, float radius, float distance, int attempts, out Vector3 clearPoint) {
        return FindBiasedRoamingPointSphereCast(source, radius, distance, new Vector2(0, 360), attempts, out clearPoint);
    }

    public static bool FindRandomRoamingPointSphereCast(Vector3 source, float radius, float distance, out Vector3 clearPoint) {
        return FindBiasedRoamingPointSphereCast(source, radius, distance, new Vector2(0, 360), out clearPoint);
    }

    private static bool AngleCastNavMesh(Vector3 source, float distance, float wallBuffer, float angle) {
        if (!AngleCast(source, distance, angle)) return false;

        Vector3 roamDir = source + AngleToXZDirection(angle) * distance;
        if (!NavMesh.SamplePosition(roamDir, out NavMeshHit sampleHit, 2f, NavMesh.AllAreas)) {
            return false;
        }

        if (NavMesh.FindClosestEdge(sampleHit.position, out NavMeshHit edgeHit, NavMesh.AllAreas)
                && edgeHit.distance < wallBuffer) {
            return false;
        }

        return true;
    }

    public static bool FindBiasedRoamingPointNavMesh(Vector3 source, float distance, Vector2 angleRange, float wallBuffer, out Vector3 clearPoint) {
        float dirAngle = Random.Range(angleRange.x, angleRange.y);
        dirAngle = SpatialUtils.WrapAngle360(dirAngle);
        Vector3 roamDir = AngleCastNavMesh(source, distance, wallBuffer, dirAngle) ? AngleToXZDirection(dirAngle)              /// Front
                        : AngleCastNavMesh(source, distance, wallBuffer, dirAngle + 180) ? AngleToXZDirection(dirAngle + 180)  /// Back
                        : AngleCastNavMesh(source, distance, wallBuffer, dirAngle + 90) ? AngleToXZDirection(dirAngle + 90)    /// Right
                        : AngleCastNavMesh(source, distance, wallBuffer, dirAngle - 90) ? AngleToXZDirection(dirAngle - 90)    /// Left
                        : AngleCastNavMesh(source, distance, wallBuffer, dirAngle + 45) ? AngleToXZDirection(dirAngle + 45)    /// Front Right
                        : AngleCastNavMesh(source, distance, wallBuffer, dirAngle - 45) ? AngleToXZDirection(dirAngle - 45)    /// Front Left
                        : AngleCastNavMesh(source, distance, wallBuffer, dirAngle + 135) ? AngleToXZDirection(dirAngle + 135)  /// Back Right
                        : AngleCastNavMesh(source, distance, wallBuffer, dirAngle - 135) ? AngleToXZDirection(dirAngle - 135)  /// Back Left
                        : Vector3.zero;                                                                                        /// Unsuccessful

        clearPoint = roamDir * distance + source;
        return roamDir.magnitude > 0;
    }

    public static bool FindBiasedRoamingPointNavMesh(Vector3 source, float distance, Vector2 angleRange,
                                                     float wallBuffer, int attempts, out Vector3 clearPoint) {
        for (int i = 0; i < attempts; i++) {
            if (FindBiasedRoamingPointNavMesh(source, distance, angleRange, wallBuffer, out clearPoint)) return true;
        }
        clearPoint = Vector3.zero;
        return false;
    }

    public static bool FindRandomRoamingPointNavMesh(Vector3 source, float distance, float wallBuffer, int attempts, out Vector3 clearPoint) {
        return FindBiasedRoamingPointNavMesh(source, distance, new Vector2(0, 360), wallBuffer, attempts, out clearPoint);
    }

    private static bool AngleCastSphere(Vector3 source, float radius, float distance, float angle) {
        Vector3 dir = AngleToXZDirection(angle);
        return !Physics.SphereCast(source, radius, dir, out _, distance, LayerUtils.EnvironmentLayerMask);
    }

    private static Vector3 AngleToXZDirection(float angle) {
        angle = SpatialUtils.WrapAngle360(angle);
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0,
                           Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
    }
}
