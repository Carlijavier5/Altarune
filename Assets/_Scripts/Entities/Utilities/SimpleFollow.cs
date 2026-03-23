using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : FollowBehavior
{
    [SerializeField] private Transform target;

    public override void Play() {
        enabled = true;
    }

    public override void Stop() {
        enabled = false;
    }

    void Update() {
        transform.position = target.position;
    }
}
