using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Miniboss {
    public class MinionController : MonoBehaviour {
        private Transform player;
        private UnityEngine.AI.NavMeshAgent navigation;

        [SerializeField] private float stoppingDistance = 1.5f;
        [SerializeField] private float speed;
        
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
}