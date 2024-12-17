using System.Linq;
using UnityEngine;

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

        if (GM.Player) player = GM.Player;
        else GM.Instance.OnPlayerInit += GM_OnPlayerInit;

        aggroRange.OnAggroEnter += AggroRange_OnAggroEnter;
        aggroRange.OnAggroExit += AggroRange_OnAggroExit;

        OnStunSet += Scaramite_OnStunSet;

        stateMachine.Init(new Scaramite_Input(stateMachine, this), new Scaramite_Roam());
    }

    private void GM_OnPlayerInit() {
        GM.Instance.OnPlayerInit -= GM_OnPlayerInit;
        player = GM.Player;
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

        if (player) {
            Vector3 playerVector = new Vector3(player.transform.position.x, 0, player.transform.position.z)
                     - new Vector3(transform.position.x, 0, transform.position.z);
            float playerAngle = playerVector.XZAngle360();

            float biasAngle = (1 - playerBias) * 180;

            float lowerBound = playerAngle - biasAngle + 360;
            float upperBound = playerAngle + biasAngle + 360;

            Vector2 angleBias = lowerBound < upperBound ? new(lowerBound, upperBound)
                                                        : new(upperBound, lowerBound);
            bool pathIsValid = PathfindingUtils.FindBiasedRoamingPoint(transform.position, distance,
                                                                       angleBias, 10, out Vector3 clearPoint);
            return pathIsValid ? clearPoint : transform.position;
        } else {
            bool pathIsValid = PathfindingUtils.FindRandomRoamingPoint(transform.position, distance,
                                                                       10, out Vector3 clearPoint);
            return pathIsValid ? clearPoint : transform.position;
        }
    }

    public void Ragdoll() {
        rb.constraints = new();
        rb.isKinematic = false;
        rb.useGravity = true;
        Vector3 force = new Vector3(Random.Range(-0.15f, 0.15f), 0.85f,
                                    Random.Range(-0.15f, 0.15f)) * Random.Range(250, 300);
        rb.AddForce(force);
        Vector3 torque = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f),
                                     Random.Range(-0.5f, 0.5f)) * Random.Range(250, 300);
        rb.AddTorque(torque);
    }

    public override void Perish(bool immediate) {
        base.Perish(immediate);
        DetachModules();

        if (immediate) {
            Destroy(gameObject);
        } else {
            enabled = false;
            aggroRange.Disable();
            Ragdoll();
            Destroy(gameObject, 2);
        }
    }
}