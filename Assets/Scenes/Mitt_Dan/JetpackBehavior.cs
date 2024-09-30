using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JetpackBehavior : Entity
{


    public float move_speed = 10f;

    


    private Vector3 randomDir;

    public float ChangeDirTime = 1f;

    public float wallAvoidanceDistance = 4f;

    private float ChangeDirTimer;


    // Start is called before the first frame update
    void Start()
    {
        randomDir = GetRandomUnitVector2D();
        ChangeDirTimer = ChangeDirTime;


            
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //FacePlayer();
        Flying();


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Entity entity)
        && entity.Faction != EntityFaction.Hostile)
        {
            entity.TryDamage(4);
        }
    }

    void Flying()
    {



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

        transform.Translate(randomDir * move_speed * Time.deltaTime);
        

        //Debug.Log(ChangeDirTimer);
    }

    Vector3 GetRandomUnitVector2D()
    {
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);
        return new Vector3(Mathf.Cos(angle), 0,Mathf.Sin(angle));
    }

}
