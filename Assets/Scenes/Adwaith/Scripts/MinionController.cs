using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MinionController : MonoBehaviour {
    public Transform player;
    public UnityEngine.AI.NavMeshAgent navigation;

    private float stoppingDistance = 1.5f;
    private float speed;
    private bool setActive = false;

    public UnityEvent onMinionDeath;

    void Start() {
        navigation = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    void Update() {
        if (player != null && setActive == true) {
            FollowPlayer();
        }
    }

    public void FollowPlayer() {
        navigation.stoppingDistance = stoppingDistance;
        navigation.autoBraking = true;
        navigation.SetDestination(player.position);
    }

    public void SetPlayer(Transform player) {
        this.player = player;
        this.navigation = navigation;
    }

    public void Activate() {
        speed = Random.Range(2f, 5f);
        navigation.speed = speed;
        gameObject.SetActive(true);
        setActive = true;
    }

    public void HandleDeath() {
        onMinionDeath.Invoke();
        gameObject.SetActive(false);
    }
}
