using System.Linq;
using UnityEngine;

/// <summary>
/// Bat Behavior
///    Bat will have 2 states: roam, chase
///    Roam state: Random movements biased towards player with objective to face the player
///    Chase state: when bat can see the player within its primary box collider, it will lock on to the player and chase
///    
/// </summary>

public partial class Scaramite : Entity {

    [SerializeField] private AggroRange aggroRange;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Rigidbody rb;

    [Header("Movement Variables")]
    [SerializeField] private float linearAcceleration = 3f;
    [SerializeField] private float linearDrag = 6f;
    [SerializeField] private float angularSpeed = 360f;

    [Header("Damage Variables")]
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float damageCD = 1f;

    private readonly StateMachine<Scaramite_Input> stateMachine = new();

    private ScaramiteLocomotionDriver driver;
    private float damageCDEndTime;

    private Player player;

    private void Awake() {
        driver = new(this, linearAcceleration, angularSpeed, linearDrag);

        aggroRange.OnAggroEnter += AggroRange_OnAggroEnter;
        aggroRange.OnAggroExit += AggroRange_OnAggroExit;

        OnStunSet += Scaramite_OnStunSet;

        player = FindAnyObjectByType<Player>();
        stateMachine.Init(new Scaramite_Input(stateMachine, this), new Scaramite_Roam());
    }

    private void Scaramite_OnStunSet(bool isStunned) {
        if (isStunned) stateMachine.SetState(new Scaramite_Stun());
        else UpdateAggro();
    }

    private void AggroRange_OnAggroEnter(Entity other) => UpdateAggro();
    private void AggroRange_OnAggroExit(Entity other) => UpdateAggro();

    private void UpdateAggro() {
        Entity closestTarget = aggroRange.ClosestTarget;
        Entity existingTarget = stateMachine.StateInput.aggroTarget;
        if (!existingTarget || !aggroRange.AggroTargets.Contains(existingTarget)) {
            stateMachine.StateInput.aggroTarget = closestTarget;
        }

        State<Scaramite_Input> nextState = closestTarget ? new Scaramite_Chase()
                                                         : new Scaramite_Roam();
        stateMachine.SetState(nextState);
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out BaseObject baseObject)
                && !baseObject.IsFaction(Faction)) {
            DealDamage(baseObject);
        }
    }

    protected override void Update() {
        base.Update();
        stateMachine.Update();
    }

    void FixedUpdate() {
        stateMachine.FixedUpdate();
    }

    private void DealDamage(BaseObject contact) {
        if (Time.time < damageCDEndTime) return;
        contact.TryDamage(damageAmount);
        damageCDEndTime = Time.time + damageCD;

        stateMachine.SetState(new Scaramite_Knockback(this, contact));
    }

    private Vector3 GetRoamDestination() {
        float distance = Random.Range(minRoamDistance, maxRoamDistance);

        Vector3 playerVector = new Vector3(player.transform.position.x, 0, player.transform.position.z)
                             - new Vector3(transform.position.x, 0, transform.position.z);
        float playerAngle = playerVector.XZAngle360();

        float biasAngle = (1 - playerBias) * 180;

        float lowerBound = playerAngle - biasAngle + 360;
        float upperBound = playerAngle + biasAngle + 360;

        Vector2 angleBias = lowerBound < upperBound ? new (lowerBound, upperBound)
                                                    : new (upperBound, lowerBound);

        bool pathIsValid = PathfindingUtils.FindBiasedRoamingPoint(transform.position, distance,
                                                                   angleBias, 10, out Vector3 clearPoint);
        return pathIsValid ? clearPoint : transform.position;
    }

    public void Ragdoll() {
        DetachModules();
        enabled = false;
        aggroRange.Disable();
        rb.constraints = new();
        rb.isKinematic = false;
        rb.useGravity = true;
        Vector3 force = new Vector3(Random.Range(-0.15f, 0.15f), 0.85f, Random.Range(-0.15f, 0.15f)) * Random.Range(250, 300);
        rb.AddForce(force);
        Vector3 torque = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)) * Random.Range(250, 300);
        rb.AddTorque(torque);
        Destroy(gameObject, 2);
    }

    public override void Perish() {
        base.Perish();
        Ragdoll();
    }
}