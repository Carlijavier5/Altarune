using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour {
    [SerializeField] private Transform follow;

    private void Update() {
        transform.position = follow.position;
    }
}
