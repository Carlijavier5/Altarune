using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(RBVelocityLimiter))]

/*
 here's a package with the ragdolling mechanic we have so far.
we have some changes in the works in the entities that will be using the ragdoll mechanic, 
so this mechanic will be localized to its own component. 
that is, we'll be a calling a ragdoll method on this component from the entity script when the entity dies, 
all the ragdolling logic should happen inside these component.

the object is guaranteed to have a rigidbody and a velocity limiter utility 
(essentially, a component that limits the max velocity a rigidbody can reach). 
the idea is to have the object ragdoll, slow down, then sink into the ground, then destroy itself. 
the objects using this will be using sphere colliders for physical collisions.

if possible, i'd use the limiter component instead of editing the rigidbody's velocity 
directly as there are a few mechanics in the game that edit that value asynchronously 
*/

public class EntityRagdoll : MonoBehaviour {
    
    // This variable references Rigibody component attached to the entity.
    [SerializeField] private Rigidbody rb;

    // This variable is a RBVelocityLimiter, limits the velocity of the RigidBody
    [SerializeField] private RBVelocityLimiter rbLimiter;

    // xzForceAmount - determines the range of force applied along x and z asix
    // torqueAmount - determines the range of torque (rotational force) applied to the entity
    [SerializeField] private float xzForceAmount = 0.15f,
                                   torqueAmount = 0.5f;

    // These Vector2 Object Variables are the max and min values for random Force and Torque applied to the Rigid Body
    [SerializeField] private Vector2 forceStrengthRange = new(250, 300),
                                     torqueStrengthRange = new(250, 300);

    // Carlos look at these variables if they're needed
    [SerializeField] private float sinkSpeed = 0.1f;        
    [SerializeField] private float ragdollDuration = 5f;
    [SerializeField] private bool isRagdoll = false;
    [SerializeField] private float maxVelocity = 10f;

    // Original Implementation
    // All the code should happen here, Ragdoll() 
    public void Ragdoll() {
        if (isRagdoll) return; // Debounce
        isRagdoll = true;

        // Resets RigidBody constraints - which removes any limits and applies random forces and torques.
        rb.constraints = new();

        // Enables gravity for the RidgidBody, it will start to fall due to gravitational force.
        rb.useGravity = true;

        // Sets the RigidBody to non-kinematic, is affected by forces gravity and collisons
        rb.isKinematic = false;

        // Here is the Random Force X is generated between the -xzForceAmnount and xzForceAmount
        // Here is the Random Force Y is generated from (1-xzForceAmount)
        // Here is the Random Force Z is generated between the -xzForceAmnount and xzForceAmount
        Vector3 force = new Vector3(Random.Range(-xzForceAmount, xzForceAmount), 1 - xzForceAmount,
                                    Random.Range(-xzForceAmount, xzForceAmount)) * Random.Range(forceStrengthRange.x,
                                                                                                forceStrengthRange.y);
        // Adds that created force to the rigidbody
        rb.AddForce(force);
        // Does the same for torque, randomly caculates a torque value in between specific bounds.
        Vector3 torque = new Vector3(Random.Range(-torqueAmount, torqueAmount), Random.Range(-torqueAmount, torqueAmount),
                                     Random.Range(-torqueAmount, torqueAmount)) * Random.Range(torqueStrengthRange.x,
                                                                                               torqueStrengthRange.y);
        // Applies that calcualted Torque to the RigidBody
        rb.AddTorque(torque);

        
        rbLimiter.MaxVelocity = maxVelocity;

        Invoke(nameof(DestroyBody), ragdollDuration);
    }

    public void Sink() {
        Vector3 downwardForce = Vector3.down * sinkSpeed;
        rb.AddForce(downwardForce, ForceMode.Acceleration);
    }

    void FixedUpdate() {
        if (isRagdoll) {
            Sink();
        }
        rbLimiter.MaxVelocity = maxVelocity;
    }

    private void DestroyBody() {
        Destroy(gameObject);
    }
}