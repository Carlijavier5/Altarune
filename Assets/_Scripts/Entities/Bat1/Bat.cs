using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Bat : Entity {

    [Header("General")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private RBVelocityLimiter limiter;
    [SerializeField] private AggroRange aggroRange;
    [SerializeField] private float wallAvoidanceDistance,
                                   changeDirTime;
    [SerializeField] int damageAmount = 4;

    private readonly StateMachine<Bat_Input> stateMachine = new();

    private float baseAnimatorSpeed;
    public float BaseAnimatorSpeed {
        get => baseAnimatorSpeed;
        set {
            baseAnimatorSpeed = value;
            animator.speed = baseAnimatorSpeed
                           * status.timeScale;
        }
    }

    void Awake() {
        OnTimeScaleSet += Bat_OnTimeScaleSet;
        OnStunSet += Bat_OnStunSet;
        OnLongPush += Bat_OnLongPush;

        baseAnimatorSpeed = animator.speed;

        stateMachine.Init(new(stateMachine, this),
                          new State_FlyIdle());
    }

    protected override void Update() {
        base.Update();
        stateMachine.Update();
    }

    void FixedUpdate() {
        stateMachine.FixedUpdate();
    }

    private void UpdateAggro() {
        if (stateMachine.State is State_Stun) return;
        if (aggroRange.HasTarget) {
            stateMachine.SetState(new State_FlyAggro());
        } else {
            stateMachine.SetState(new State_FlyIdle());
        }
    }

    private Vector3 GetRandomDirection(Vector3 prevDirection) {
        if (PathfindingUtils.FindRandomRoamingPoint(transform.position, wallAvoidanceDistance,
                                                    10, out Vector3 clearPoint)) {
            Vector3 dir = clearPoint - transform.position;
            dir.y = 0;
            return dir.normalized;
        } return prevDirection;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out BaseObject baseObject)
                && !baseObject.IsFaction(EntityFaction.Hostile)) {
            if (baseObject.TryDamage(damageAmount)) {
                Vector3 pushDir = other.transform.position - transform.position;
                if (TryLongPush(pushDir, knockbackStrength, knockbackDuration,
                                out PushActionCore actionCore)) {
                    actionCore.SetEase(EaseCurve.Logarithmic);
                } ApplyEffects(new[] { new StunStatusEffect(stunDuration) });
            }
        }
    }

    private void Bat_OnTimeScaleSet(float timeScale) {
        animator.speed = baseAnimatorSpeed * timeScale;
    }

    private void Bat_OnStunSet(bool isStunned) {
        if (isStunned) stateMachine.SetState(new State_Stun());
        else stateMachine.SetState(new State_FlyIdle());
    }

    private void Bat_OnLongPush() {
        ApplyEffects(new[] { new StunStatusEffect(stunDuration) });
    }

    public override void Perish() {
        base.Perish();
        DetachModules();
        enabled = false;
        aggroRange.Disable();
        Ragdoll();
        Destroy(gameObject, 2);
    }

    private void Ragdoll() {
        rb.constraints = new();
        rb.useGravity = true;
        rb.isKinematic = false;
        Vector3 force = new Vector3(Random.Range(-0.15f, 0.15f), 0.85f,
                                    Random.Range(-0.15f, 0.15f)) * Random.Range(250, 300);
        rb.AddForce(force);
        Vector3 torque = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f),
                                     Random.Range(-0.5f, 0.5f)) * Random.Range(250, 300);
        rb.AddTorque(torque);
    }
}

public partial class Bat {

    public class Bat_Input : StateInput {

        public StateMachine<Bat_Input> stateMachine;
        public Bat bat;

        public Bat_Input(StateMachine<Bat_Input> stateMachine, Bat bat) {
            this.stateMachine = stateMachine;
            this.bat = bat;
        }
    }
}

public partial class Bat {

    public abstract class State_Fly : State<Bat_Input> {

        protected abstract float MoveSpeed { get; }

        protected Bat bat;
        private Vector3 moveDir;
        private float changeDirTimer;

        public override void Enter(Bat_Input input) {
            bat = input.bat;
            moveDir = bat.GetRandomDirection(moveDir);
            changeDirTimer = bat.changeDirTime;
        }

        public override void Update(Bat_Input input) { }

        public override void FixedUpdate(Bat_Input input) {
            if (changeDirTimer <= 0) {
                moveDir = bat.GetRandomDirection(moveDir);
                changeDirTimer = bat.changeDirTime;
            }

            if (Physics.Raycast(bat.transform.position, moveDir,
                                bat.wallAvoidanceDistance,
                                LayerUtils.EnvironmentLayerMask)) {
                moveDir = bat.GetRandomDirection(moveDir);
                changeDirTimer = bat.changeDirTime;
            }

            changeDirTimer -= bat.DeltaTime;

            bat.rb.AddForce(bat.TimeScale * bat.RootMult * MoveSpeed
                            * moveDir, ForceMode.Force);
            bat.transform.LookAt(bat.transform.position + bat.rb.velocity);
        }

        public override void Exit(Bat_Input input) { }
    }
}

public partial class Bat {

    [Header("Idle")]
    [SerializeField] private float idleMoveSpeed;
    [SerializeField] private float idleMaxVelocity,
                                   idleAnimationSpeed,
                                   aggroUpdateInterval;

    public class State_FlyIdle : State_Fly {

        protected override float MoveSpeed => bat.idleMoveSpeed;
        private float aggroUpdateTimer;

        public override void Enter(Bat_Input input) {
            base.Enter(input);
            bat.BaseAnimatorSpeed = bat.idleAnimationSpeed;
            aggroUpdateTimer = bat.aggroUpdateInterval;
            bat.limiter.MaxVelocity = bat.idleMaxVelocity;
        }

        public override void Update(Bat_Input input) {
            aggroUpdateTimer -= bat.DeltaTime;
            if (aggroUpdateTimer <= 0) bat.UpdateAggro();
        }
    }
}

public partial class Bat {

    [Header("Aggro")]
    [SerializeField] private float aggroMoveSpeed;
    [SerializeField] private float aggroMaxVelocity,
                                   aggroAnimationSpeed,
                                   aggroPacifyTime;
                                    
    public class State_FlyAggro : State_Fly {

        protected override float MoveSpeed => bat.aggroMoveSpeed;
        private float pacifyTimer;

        public override void Enter(Bat_Input input) {
            base.Enter(input);
            bat.BaseAnimatorSpeed = bat.aggroAnimationSpeed;
            pacifyTimer = bat.aggroPacifyTime;
            bat.limiter.MaxVelocity = bat.aggroMaxVelocity;
        }

        public override void Update(Bat_Input input) {
            pacifyTimer -= bat.DeltaTime;
            if (pacifyTimer <= 0) {
                input.stateMachine.SetState(new State_FlyIdle());
            }
        }
    }
}

public partial class Bat {

    [Header("Knockback")]
    [SerializeField] private float knockbackStrength;
    [SerializeField] private float knockbackDuration,
                                   stunDuration;

    public class State_Stun : State<Bat_Input> {

        public override void Enter(Bat_Input _) { }

        public override void Update(Bat_Input _) { }

        public override void FixedUpdate(Bat_Input input) {
            input.bat.rb.AddForce(-input.bat.rb.velocity, ForceMode.Force);
        }

        public override void Exit(Bat_Input _) { }
    }
}