using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaConnection : MonoBehaviour {

    [SerializeField] private LineRenderer lrOrigin, lrTail;
    [SerializeField] private float connectDuration, disconnectDuration,
                                   floorOffset, width;
    private Transform origin, tail;

    private bool isActive;

    void Awake() {
        lrOrigin.positionCount = 2;
        lrTail.positionCount = 2;

        lrOrigin.startWidth = width;
        lrOrigin.endWidth = width;

        lrTail.startWidth = width;
        lrTail.endWidth = width;
    }

    public void Init(Transform origin, Transform tail) {
        this.origin = origin;
        this.tail = tail;
        isActive = true;
        Connect();
    }

    public void Connect() {
        if (isActive) {
            StopAllCoroutines();
            StartCoroutine(IConnect());
        }
    }

    public void Disconnect(bool terminate = false) {
        StopAllCoroutines();
        if (terminate) isActive = false;
        StartCoroutine(IDisconnect());
    }

    private IEnumerator IConnect() {
        Vector3 originPoint = new(origin.position.x,
                                  origin.position.y + floorOffset,
                                  origin.position.z);
        Vector3 tailPoint = new(tail.position.x,
                                tail.position.y + floorOffset,
                                tail.position.z);
        Vector3 midPoint = Vector3.Lerp(originPoint, tailPoint, 0.5f);

        lrOrigin.SetPosition(0, originPoint);
        lrTail.SetPosition(0, tailPoint);

        float timer = 0;
        Vector3 lrOPos, lrTPos;
        float lerpVal;
        while (timer < connectDuration) {
            timer = Mathf.MoveTowards(timer, connectDuration, Time.deltaTime);
            lerpVal = timer / connectDuration;
            lrOPos = Vector3.Lerp(originPoint, midPoint, lerpVal);
            lrTPos = Vector3.Lerp(tailPoint, midPoint, lerpVal);
            lrOrigin.SetPosition(1, lrOPos);
            lrTail.SetPosition(1, lrTPos);
            yield return null;
        }
    }

    private IEnumerator IDisconnect() {
        Vector3 originPoint = new(origin.position.x,
                          origin.position.y + floorOffset,
                          origin.position.z);
        Vector3 tailPoint = new(tail.position.x,
                                tail.position.y + floorOffset,
                                tail.position.z);
        Vector3 midPoint = Vector3.Lerp(originPoint, tailPoint, 0.5f);

        float timer = 0;
        Vector3 lrOPos, lrTPos;
        float lerpVal;
        while (timer < disconnectDuration) {
            timer = Mathf.MoveTowards(timer, disconnectDuration, Time.deltaTime);
            lerpVal = timer / disconnectDuration;
            lrOPos = Vector3.Lerp(originPoint, midPoint, lerpVal);
            lrTPos = Vector3.Lerp(tailPoint, midPoint, lerpVal);
            lrOrigin.SetPosition(0, lrOPos);
            lrTail.SetPosition(0, lrTPos);
            yield return null;
        }

        if (!isActive) Destroy(gameObject, 0.1f);
    }
}
