using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlitherZig : MonoBehaviour {

    public event System.Action OnWarningComplete;

    [SerializeField] private SlitherZigHitbox zigHitbox1, zigHitbox2,
                                              zigHitbox3;
    [SerializeField] private float hitboxShowDelay, zigCooldown;

    public readonly SlitherZigSegment[] segments = new SlitherZigSegment[3];

    public bool IsAvailable => timer <= 0;
    private float timer;

    void Awake() {
        transform.SetParent(null);
    }

    public void DoZig(Vector3 sourcePos, Vector3 targetPos) {
        sourcePos.y = transform.position.y; targetPos.y = transform.position.y;

        Vector3 distanceToTargetExtended = (targetPos - sourcePos) * 2f;

        Vector3 distance1 = sourcePos + distanceToTargetExtended / 3.0f;
        Vector3 distance2 = sourcePos + (2.0f * distanceToTargetExtended / 3.0f);
        Vector3 distance3 = sourcePos + (3.0f * distanceToTargetExtended / 3.0f);

        Vector3 offset1 = new(distanceToTargetExtended.z / 3.0f, 0, -distanceToTargetExtended.x / 3.0f);
        Vector3 offset2 = new(-distanceToTargetExtended.z / 3.0f, 0, distanceToTargetExtended.x / 3.0f);
        Vector3 offset3 = new(distanceToTargetExtended.z / 3.0f, 0, -distanceToTargetExtended.x / 3.0f);

        Vector3 position1 = distance1 + offset1;
        Vector3 position2 = distance2 + offset2;
        Vector3 position3 = distance3 + offset3;

        StartCoroutine(IDoCooldown());
        StartCoroutine(IDoZig(sourcePos, position1, position2, position3));
    }

    private IEnumerator IDoZig(Vector3 originalPosition, Vector3 newPosition1,
                               Vector3 newPosition2, Vector3 newPosition3) {
        Vector3 yOffset = new(0, transform.position.y, 0);

        Vector3 position;
        Quaternion rotation;
        float zScale;

        segments[0] = new SlitherZigSegment(originalPosition, newPosition1,
                                            zigHitbox1);

        position = (originalPosition + newPosition1) / 2.0f + yOffset;
        rotation = Quaternion.LookRotation(newPosition1 - originalPosition);
        zScale = (newPosition1 - originalPosition).magnitude;
        zigHitbox1.Generate(position, rotation, zScale, newPosition1);

        yield return new WaitForSeconds(hitboxShowDelay);

        segments[1] = new SlitherZigSegment(newPosition1, newPosition2,
                                            zigHitbox2);

        position = (newPosition1 + newPosition2) / 2.0f + yOffset;
        rotation = Quaternion.LookRotation(newPosition2 - newPosition1);
        zScale = (newPosition2 - newPosition1).magnitude;
        zigHitbox2.Generate(position, rotation, zScale, newPosition2);

        yield return new WaitForSeconds(hitboxShowDelay);

        segments[2] = new SlitherZigSegment(newPosition2, newPosition3,
                                            zigHitbox3);

        position = (newPosition2 + newPosition3) / 2.0f + yOffset;
        rotation = Quaternion.LookRotation(newPosition3 - newPosition2);
        zScale = (newPosition3 - newPosition2).magnitude;
        zigHitbox3.Generate(position, rotation, zScale, newPosition3);

        yield return new WaitForSeconds(hitboxShowDelay);

        OnWarningComplete?.Invoke();
    }

    public void CancelZig() {
        zigHitbox1.Deactivate();
        zigHitbox2.Deactivate();
        zigHitbox3.Deactivate();
    }

    private IEnumerator IDoCooldown() {
        timer = zigCooldown;
        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }
    }
}