using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoSkill : BasePlayerSkill
{
    [Header("Tornado Parameters")]
    [SerializeField] private float pushStrength;
    [SerializeField] private float rotationSpeed;

    private List<BaseObject> affectedObjects = new List<BaseObject>();

    private GameObject tornadoVFX;
    private float scaleDuration = 1.5f;
    [SerializeField] private AnimationCurve scaleCurve;

    public override void SpawnSkill(PlayerSkillData data, Vector3 playerPos, Vector3 targetPos, ISkillSpawn spawnBehavior, ISkillMovement moveBehavior) {
        base.SpawnSkill(data, playerPos, targetPos, new DefaultSpawn(), new ProjectileMovement());

        //StartCoroutine(ScaleTornado());
    }

    public override void MoveSkill() {
        base.MoveSkill();

        // am also going to apply tornado force here bc im lazy and we only have 3 skills so FCK
        // why did i try and make this scalable in the first place
        foreach (BaseObject obj in affectedObjects) {
            // Debug.Log(obj.name + " go burr");
            ApplyTornadoForce(obj);
        }
    }

    private void ApplyTornadoForce(BaseObject obj) {
        // calculate direction to simulate circular motion
        // i know that grabbing these transforms is dumb as sh*t for optimization but owell
        Vector3 directionToCenter = (obj.transform.position - transform.position).normalized;
        Vector3 circularForceDirection = Vector3.Cross(Vector3.up, directionToCenter);

        // combine outward push with circular motion
        Vector3 pushDirection = (directionToCenter + circularForceDirection * rotationSpeed);
        pushDirection.y = 0;
        pushDirection = pushDirection.normalized;

        // push the object
        obj.TryPush(-pushDirection, pushStrength * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out BaseObject obj)) {
            affectedObjects.Add(obj);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out BaseObject obj)) {
            affectedObjects.Remove(obj);
        }
    }

    //// for scaling the tornado
    //private IEnumerator ScaleTornado() {
    //    float t = 0f;
    //    while (t < scaleDuration) {
    //        t += Time.deltaTime;

    //        float delta = scaleCurve.Evaluate(t);
    //        UpdateTornadoScale(delta);
    //        yield return null;
    //    }
    //}

    //private void UpdateTornadoScale(float val) {
    //    transform.localScale *= val;
    //}
}
