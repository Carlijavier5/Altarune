using System.Collections.Generic;
using UnityEngine;

namespace FeatureSamples {

    /// <summary>
    /// Sample class that propagates sample CC ailments to neighboring entities;
    /// </summary>
    public class SampleCCSource : MonoBehaviour {

        [Header("Root (R)")]
        [SerializeField] private float rootDuration;
        [Header("Stun (T)")]
        [SerializeField] private float stunDuration;
        [Header("Slow (F)")]
        [SerializeField] private float slowDuration;
        [Range(0.1f, 10f)][SerializeField] private float speedMultiplier;
        [Header("Stagger (G)")]
        [SerializeField] private float staggerDuration;

        private readonly HashSet<Entity> entities = new();

        void Update() {

            if (Input.GetKeyDown(KeyCode.R)) {
                foreach (Entity entity in entities) {
                    if (entity) entity.ApplyEffects(new[] { new SampleRootEffect(rootDuration) });
                }
            }

            if (Input.GetKeyDown(KeyCode.T)) {
                foreach (Entity entity in entities) {
                    if (entity) entity.ApplyEffects(new[] { new SampleStunEffect(stunDuration) });
                }
            }

            if (Input.GetKeyDown(KeyCode.F)) {
                foreach (Entity entity in entities) {
                    if (entity) entity.ApplyEffects(new[] { new SampleSlowEffect(slowDuration,
                                                                                 speedMultiplier) });
                }
            }

            if (Input.GetKeyDown(KeyCode.G)) {
                foreach (Entity entity in entities) {
                    if (entity) entity.TryStagger(staggerDuration);
                }
            }
        }

        void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out Entity entity)) {
                entities.Add(entity);
            }
        }
    }
}