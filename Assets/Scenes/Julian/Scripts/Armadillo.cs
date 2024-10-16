using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.AI;




public partial class Armadillo : Entity {
    [Header("Movement")]
    [SerializeField] private Vector2 idleWaitTimeRange;
    [SerializeField] private Vector2 roamDistanceRange;
    [SerializeField] private Vector2 rollWaitTimeRange;
    [SerializeField] private float rollSpeed;
    [SerializeField] private float roamWallBuffer, roamSpeed, roamAngularSpeed, roamAcceleration;

    [SerializeField] private NavMeshAgent navMeshAgent;
    private ArmadilloState state;
    private bool inAggroRange = false;
    [SerializeField] private ParticleSystem rollParticleSystem;

    [SerializeField] private AggroRange aggroRange;
    private Entity aggroEntity;
    private float agitation = 1.0f;
    private float Agitation {
        get => agitation;
        set {
            agitation = value;
            UpdateNavMeshSpeeds();
        }
    }

    public void UpdateNavMeshSpeeds() {
        navMeshAgent.speed = roamSpeed * status.timeScale * Agitation;
        navMeshAgent.angularSpeed = roamAngularSpeed * status.timeScale * Agitation;
    }

    private void SetState(ArmadilloState newState) {
        state.Exit(this);
        state = newState;
        newState.Enter(this);
    }
    public void CalmDown() {
        if (Agitation > 1) {
            Agitation -= Time.deltaTime * TimeScale * 0.1f;
            if (Agitation < 1) Agitation = 1;
        }
    }


    private void Start() {
        OnTimeScaleSet += Armadillo_OnTimeScaleSet;
        OnTryDamage += HandleTryDamage;

        navMeshAgent.speed = roamSpeed;
        navMeshAgent.acceleration = roamAcceleration;
        navMeshAgent.autoBraking = true;

        state = new ArmadilloIdleState();
        state.Enter(this);
        rollParticleSystem.Stop();

        aggroRange.OnAggroEnter += AggroRange_OnAggroEnter;
        aggroRange.OnAggroExit += AggroRange_OnAggroExit;

    }

    private void Armadillo_OnTimeScaleSet(float timeScale) => UpdateNavMeshSpeeds();

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Entity entity)
            && entity.Faction != EntityFaction.Hostile) {
            entity.TryDamage(4);
        }
    }
    public override void Perish() {
        base.Perish();
        Destroy(gameObject);
    }
    protected override void Update() {
        base.Update();
        state.Update(this);
        CleanUpOldDamageEvents();
    }
    private abstract record ArmadilloState {

        public abstract void Enter(Armadillo armadillo);

        public abstract void Update(Armadillo armadillo);

        public abstract void Exit(Armadillo armadillo);
    }
}

