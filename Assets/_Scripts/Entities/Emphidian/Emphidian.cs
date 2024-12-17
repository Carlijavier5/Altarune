using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public partial class Emphidian : Entity {

    private const string WALK_SPEED_PARAM = "WalkSpeed";

    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Rigidbody rb;
    private readonly StateMachine<Emphidian_Input> stateMachine = new();

    private float baseLinearSpeed;
    private float baseAngularSpeed;

    private int speedParam;

    void Awake() {
        OnTimeScaleSet += Emphidian_OnTimeScaleSet;
        OnRootSet += Emphidian_OnRootSet;
        OnStunSet += Emphidian_OnStunSet;

        baseLinearSpeed = navMeshAgent.speed;
        baseAngularSpeed = navMeshAgent.angularSpeed;

        Player player = FindObjectOfType<Player>();
        stateMachine.Init(new(stateMachine, this), new State_Roam());
        stateMachine.StateInput.SetTarget(player);
        speedParam = Animator.StringToHash(WALK_SPEED_PARAM);
    }

    private void Emphidian_OnTimeScaleSet(float timeScale) {
        navMeshAgent.speed = baseLinearSpeed
                           * timeScale
                           * RootMult;
        navMeshAgent.angularSpeed = baseAngularSpeed
                                  * timeScale
                                  * RootMult;
        animator.speed = timeScale;
    }

    private void Emphidian_OnRootSet(bool _) {
        navMeshAgent.speed = baseLinearSpeed
                           * status.timeScale
                           * RootMult;
    }
    private void Emphidian_OnStunSet(bool isStunned) {
        if (isStunned) stateMachine.SetState(new State_Stun());
        else stateMachine.SetState(new State_Roam());
    }

    protected override void Update() {
        base.Update();
        stateMachine.Update();
        animator.SetFloat(speedParam, navMeshAgent.velocity.magnitude
                                      / Mathf.Max(1, baseLinearSpeed));
    }

    private Vector3 GetRoamDestination() {
        float distance = Random.Range(roamDistanceRange.x, roamDistanceRange.y);

        Vector3 clearPoint;
        bool pathIsValid;
        if (stateMachine.StateInput.aggroTarget) {
            Entity target = stateMachine.StateInput.aggroTarget;

            Vector3 targetVector = new Vector3(target.transform.position.x, 0, target.transform.position.z)
                                 - new Vector3(transform.position.x, 0, transform.position.z);
            float targetAngle = targetVector.XZAngle360();

            float biasAngle = (1 - playerBias) * 180;

            float lowerBound = targetAngle - biasAngle + 360;
            float upperBound = targetAngle + biasAngle + 360;

            Vector2 angleBias = lowerBound < upperBound ? new(lowerBound, upperBound)
                                                        : new(upperBound, lowerBound);

            pathIsValid = PathfindingUtils.FindBiasedRoamingPoint(transform.position, distance,
                                                                  angleBias, 10, out clearPoint);
        } else {
            pathIsValid = PathfindingUtils.FindRandomRoamingPoint(transform.position, distance,
                                                                  10, out clearPoint);
        } return pathIsValid ? clearPoint : transform.position;
    }
    
    public void Animator_ReleaseProjectile() {
        Quaternion lookRotation = Quaternion.LookRotation(lockedShootDirection, Vector3.up);
        Instantiate(projectilePrefab, shootPoint.position, lookRotation);
    }

    private void Ragdoll() {
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
            Ragdoll();
            Destroy(gameObject, 2);
        }
    }
}