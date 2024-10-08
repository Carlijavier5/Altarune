using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;




public class Armadillo : Entity
{
    [Header("Movement")]
    [SerializeField] private Vector2 idleWaitTimeRange;
    [SerializeField] private Vector2 roamDistanceRange;
    [SerializeField] private Vector2 rollWaitTimeRange;
    [SerializeField] private float rollSpeed;
    [SerializeField] private float roamWallBuffer, roamSpeed, roamAngularSpeed, roamAcceleration;

    [SerializeField] private NavMeshAgent navMeshAgent;
    private ArmadilloState state;
    public bool inAggroRange = false;
    [SerializeField] private ParticleSystem rollParticleSystem;

    [SerializeField] private AggroRange aggroRange;
    private Entity aggroEntity;
    private float agitation = 1.0f;
    private float Agitation
    {
        get => agitation;
        set
        {
            agitation = value;
            UpdateNavMeshSpeeds();
        }
    }
    public override float TimeScale
    {
        get => base.TimeScale;
        set
        {
            timeScale = value;
            UpdateNavMeshSpeeds();
        }
    }
    public void UpdateNavMeshSpeeds()
    {
        navMeshAgent.speed = roamSpeed * timeScale * Agitation;
        navMeshAgent.angularSpeed = roamAngularSpeed * timeScale * Agitation;
    }

    private void SetState(ArmadilloState newState)
    {
        state.Exit(this);
        state = newState;
        newState.Enter(this);
    }
    public void CalmDown()
    {
        if (Agitation > 1)
        {
            Agitation -= Time.deltaTime * TimeScale * 0.1f;
            if (Agitation < 1) Agitation = 1;
        }
    }


    private void Start()
    {
        navMeshAgent.speed = roamSpeed;
        navMeshAgent.acceleration = roamAcceleration;
        navMeshAgent.autoBraking = true;

        OnTryDamage += HandleTryDamage;

        state = new ArmadilloIdleState();
        state.Enter(this);
        rollParticleSystem.Stop();

        aggroRange.OnAggroEnter += AggroRange_OnAggroEnter;
        aggroRange.OnAggroExit += AggroRange_OnAggroExit;

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Entity entity)
            && entity.Faction != EntityFaction.Hostile)
        {
            entity.TryDamage(4);
        }
    }

    public override void Perish()
    {
        base.Perish();
        Destroy(gameObject);
    }



    protected override void Update()
    {
        base.Update();
        state.Update(this);
        CleanUpOldDamageEvents();
    }


    private abstract record ArmadilloState
    {

        public abstract void Enter(Armadillo armadillo);

        public abstract void Update(Armadillo armadillo);

        public abstract void Exit(Armadillo armadillo);
    }
    private record ArmadilloRoamState : ArmadilloState
    {
        public override void Enter(Armadillo armadillo)
        {
            armadillo.navMeshAgent.enabled = true;
            SetNewRoamTarget(armadillo);
        }
        public override void Update(Armadillo armadillo)
        {
            armadillo.CalmDown();
            if (armadillo.navMeshAgent.remainingDistance <= Mathf.Epsilon)
            {
                armadillo.SetState(new ArmadilloIdleState());
            }
            armadillo.MaybeRollUp();
        }
        private void SetNewRoamTarget(Armadillo armadillo)
        {

            Transform t = armadillo.transform;
            Vector3 dir = new(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            Vector2 distanceRange = armadillo.roamDistanceRange;
            float distance = Random.Range(distanceRange.x, distanceRange.y) * armadillo.Agitation;

            int depthLimit = 0;
            Vector3 targetLocation = Vector3.zero;

            while (targetLocation == Vector3.zero && depthLimit < 10)
            {
                Ray ray = new(t.position, dir);
                IEnumerable<RaycastHit> info = Physics.RaycastAll(ray, distance + armadillo.roamWallBuffer)
                                                      .Where((info) => !info.collider.isTrigger);
                if (info.Count() == 0)
                {
                    targetLocation = ray.GetPoint(distance);
                }
                depthLimit++;
            }
            if (depthLimit >= 10)
            {
                armadillo.SetState(new ArmadilloIdleState());
                return;
            }
            armadillo.navMeshAgent.SetDestination(targetLocation);
        }
        public override void Exit(Armadillo armadillo) { armadillo.navMeshAgent.enabled = false; }
    }
    private record ArmadilloIdleState : ArmadilloState
    {
        private float timeRemaining = 0;
        public override void Enter(Armadillo armadillo)
        {
            timeRemaining = Random.Range(armadillo.idleWaitTimeRange.x, armadillo.idleWaitTimeRange.y) / armadillo.Agitation * armadillo.TimeScale;
        }

        public override void Update(Armadillo armadillo)
        {
            armadillo.CalmDown();
            timeRemaining -= Time.deltaTime * armadillo.TimeScale;


            if (armadillo.inAggroRange)
            {
                armadillo.SetState(new ArmadilloAggroState());
                return;
            }

            if (timeRemaining <= 0)
            {
                armadillo.SetState(new ArmadilloRoamState());
                return;
            }
            armadillo.MaybeRollUp();
        }

        public override void Exit(Armadillo armadillo) { }

    }
    private record ArmadilloAggroState : ArmadilloState
    {
        public override void Enter(Armadillo armadillo)
        {
            armadillo.Agitation = 1.75f;
            armadillo.navMeshAgent.enabled = true;
        }

        public override void Update(Armadillo armadillo)
        {
            armadillo.navMeshAgent.SetDestination(armadillo.aggroEntity.transform.position);
            if (armadillo.inAggroRange == false)
            {
                armadillo.SetState(new ArmadilloRoamState());
            }
        }

        public override void Exit(Armadillo armadillo) { armadillo.navMeshAgent.enabled = false; }
    }

    private record ArmadilloRollState : ArmadilloState
    {
        private RollStatusEffect effect;
        private float timeRemaining;
        private Vector3 randomDirection;
        public override void Enter(Armadillo armadillo)
        {
            timeRemaining = Random.Range(armadillo.rollWaitTimeRange.x, armadillo.rollWaitTimeRange.y) * armadillo.TimeScale;
            effect = armadillo.rollStatusEffect.Clone() as RollStatusEffect;
            armadillo.ApplyEffects(new[] { effect });

            float angle = Random.Range(0f, 360f);  // Random angle in degrees
            randomDirection = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));

            armadillo.navMeshAgent.enabled = true;

            armadillo.rollParticleSystem.Play();
        }

        public override void Update(Armadillo armadillo)
        {
            timeRemaining -= Time.deltaTime * armadillo.TimeScale;
            if (timeRemaining <= 0)
            {
                armadillo.SetState(new ArmadilloRoamState());
                return;
            }
            armadillo.navMeshAgent.speed = armadillo.rollSpeed * armadillo.TimeScale;
            armadillo.navMeshAgent.SetDestination(armadillo.transform.position + randomDirection * 10);
            armadillo.IgnoreDamageEvents();
        }

        public override void Exit(Armadillo armadillo)
        {
            effect.Stop();
            armadillo.UpdateNavMeshSpeeds();
            armadillo.navMeshAgent.enabled = false;
            armadillo.rollParticleSystem.Stop();

        }
    }


    // Struct to store damage event information
    private struct DamageEvent
    {
        public int damageAmount;
        public float time;

        public DamageEvent(int amount, float time)
        {
            this.damageAmount = amount;
            this.time = time;
        }
    }

    [Header("Rolling")]
    // List to store the damage events
    private List<DamageEvent> damageEvents = new List<DamageEvent>();

    // Time window to track damage (5 seconds)
    [SerializeField]
    private float damageTrackingWindow = 5f;

    [SerializeField]
    private float rollThresholdDamage = 5;

    [SerializeField]
    public RollStatusEffect rollStatusEffect;


    private void CleanUpOldDamageEvents()
    {
        float currentTime = Time.time;

        // Remove any damage event that is older than 5 seconds
        damageEvents.RemoveAll(eventData => currentTime - eventData.time > damageTrackingWindow);
    }

    private void IgnoreDamageEvents()
    {
        damageEvents.Clear();
    }

    public void MaybeRollUp()
    {
        int damageSum = 0;
        foreach (DamageEvent ev in damageEvents)
        {
            damageSum += ev.damageAmount;
        }
        Debug.Log(damageSum);
        if (damageSum > rollThresholdDamage)
        {
            SetState(new ArmadilloRollState());
        }
    }

    void HandleTryDamage(int amount, ElementType element, EventResponse response)
    {
        damageEvents.Add(new DamageEvent(amount, Time.time));
    }
    private void AggroRange_OnAggroEnter(Entity entity)
    {
        inAggroRange = true;
        aggroEntity = entity;
    }

    private void AggroRange_OnAggroExit(Entity entity)
    {
        inAggroRange = false;
    }

}

