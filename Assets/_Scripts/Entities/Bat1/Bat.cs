using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Entity
{
    [SerializeField] private Rigidbody rb;

    [SerializeField] float move_speed = 10f;

    private Vector3 randomDir;

    [SerializeField] float ChangeDirTime = 1f;

    [SerializeField] float wallAvoidanceDistance = 4f;

    private float ChangeDirTimer;

    [SerializeField] int damage = 4;


    // Start is called before the first frame update
    private void Start() {
        randomDir = GetRandomUnitVector2D();
        ChangeDirTimer = ChangeDirTime;



    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        //FacePlayer();
        Flying();


    }
    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity)
            && entity.Faction != EntityFaction.Hostile)
        {
            entity.TryDamage(damage);
        }
    }

    private void Flying() {

        if (ChangeDirTimer < 0)
        {
            randomDir = GetRandomUnitVector2D();
            ChangeDirTimer = ChangeDirTime;
        }

        if (Physics.Raycast(transform.position, randomDir, out RaycastHit hitInfo, wallAvoidanceDistance))
        {
            int hitObjectLayer = hitInfo.collider.gameObject.layer;
            if (LayerMask.LayerToName(hitObjectLayer) != "Player")
            {
                randomDir = GetRandomUnitVector2D(); // Change direction if about to hit a wall
                ChangeDirTimer = ChangeDirTime;
            }

        }

        ChangeDirTimer -= Time.deltaTime;

        rb.AddForce(randomDir * move_speed * Time.deltaTime, ForceMode.Force);
        transform.LookAt(transform.position + rb.velocity);


        //Debug.Log(ChangeDirTimer);
    }

    private Vector3 GetRandomUnitVector2D() {
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);
        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
    }


    public override void Perish() {
        base.Perish();
        Ragdoll();
    }

    private void Ragdoll() {
        rb.constraints = new();
        rb.useGravity = true;
        rb.isKinematic = false;
        Vector3 force = new Vector3(Random.Range(-0.15f, 0.15f), 0.85f, Random.Range(-0.15f, 0.15f)) * Random.Range(250, 300);
        rb.AddForce(force);
        Vector3 torque = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)) * Random.Range(250, 300);
        rb.AddTorque(torque);
        DetachModules();
        enabled = false;
        Destroy(gameObject, 2);
    }
}
