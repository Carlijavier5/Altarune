using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BaseObject {

    /// <summary>
    /// Called to request teleportation from a Teleport Module;
    /// </summary>
    public System.Action<Vector3, EventResponse<Vector3>> OnTryTeleport;

    /// <summary>
    /// Called when a teleport action begins;
    /// Passes the target teleport position;
    /// </summary>
    public System.Action<Vector3> OnTeleportStart;
    /// <summary>
    /// Called when a teleport action has ended;
    /// </summary>
    public System.Action OnTeleportEnd;

    /// <summary>
    /// Teleport the object to the nearest valid position
    /// to the position provided;
    /// </summary>
    /// <param name="targetPosition"> Desired teleport position; </param>
    /// <param name="processedPosition"> Nearest valid position; </param>
    /// <returns> True if the object is teleportable, false otherwise; </returns>
    public bool TryTeleport(Vector3 targetPosition,
                            out Vector3 processedPosition) {
        EventResponse<Vector3> response = new();
        OnTryTeleport?.Invoke(targetPosition, response);
        processedPosition = response.objectReference;
        return response.received;
    }

    /// <summary>
    /// Teleport the object to the nearest valid position
    /// to the position provided;
    /// </summary>
    /// <param name="targetPosition"> Desired teleport position; </param>
    /// <returns> True if the object is teleportable, false otherwise; </returns>
    public bool TryTeleport(Vector3 targetPosition) {
        return TryTeleport(targetPosition, out Vector3 _);
    }

    public void ConfirmTeleportStart(Vector3 teleportPosition) {
        OnTeleportStart?.Invoke(teleportPosition);
    }

    public void ConfirmTeleportEnd() => OnTeleportEnd?.Invoke(); 
}