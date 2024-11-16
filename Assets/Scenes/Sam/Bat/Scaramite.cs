using UnityEngine;

/// <summary>
/// Bat Behavior
///    Bat will have 2 states: roam, chase
///    Roam state: Random movements biased towards player with objective to face the player
///    Chase state: when bat can see the player within its primary box collider, it will lock on to the player and chase
///    
/// </summary>

public class Scaramite : Entity {

    [SerializeField] private Collider visionCollider;
    [SerializeField] private AggroRange aggroRange;
    [SerializeField] private ScaramiteBodyTrigger attackTrigger;

    [SerializeField] private CharacterController controller;
    [SerializeField] private Rigidbody ayayay;

    [Header("Movement Variables")]
    [SerializeField] private float linearAcceleration = 3f;
    [SerializeField] private float angularSpeed = 360f;
    [SerializeField] private float playerBias = 0.65f;

    [Header("Roam Variables")]
    [SerializeField] private float timeBetweenMove = 0.35f;
    [SerializeField] private float roamSpeed = 3.5f;
    [SerializeField] private float minRoamDistance;
    [SerializeField] private float maxRoamDistance;
    [SerializeField] private float stoppingDistance = 0.1f;

    [Header("Chase Variables")]
    [SerializeField] private float chaseSpeed = 5f;

    [Header("Damage Variables")]
    [SerializeField] private float damageAmount = 1;
    [SerializeField] private float damageCD = 1f;

    [Header("Self-Knockback Variables")]
    [SerializeField] private float knockbackForce = 2f;
    [SerializeField] private float knockbackDuration = 0.8f;

    private float speed;
    private Vector3 destination;
    private bool isKnockedBack;

    private float timer;
    private float damageCDEndTime;
    private float knockbackEndTime;

    private Player player; //Will store the player that triggered the chase

    private enum State {
        Roam,
        Chase
    }

    private State state = State.Roam;
    
    void Start() {
        destination = GetRoamDestination(); //Begin movement pattern towards the player
        player = FindAnyObjectByType<Player>();
    }

    protected override void Update() {
        base.Update();
        if (isKnockedBack) { //Don't want to call move commands if being knocked back
            if (Time.time >= knockbackEndTime) {
                isKnockedBack = false;
                attackTrigger.SetCanAttack(true); //Bat can attack again after knockback
            } else {
                return;
            }
        }
    }

    void FixedUpdate() {
        switch (state) {
            case State.Roam:
                Roam();
                break;
            case State.Chase:
                Chase();
                break;
        }
    }

    //Detects if the player has entered the scaramite's vision and triggers chase mode
    void OnTriggerEnter(Collider other) {
        if (state == State.Chase) return;
        else if (other.TryGetComponent(out Player player)) {
            this.player = player;
            SetState(State.Chase);
        }
    }

    private void SetState(State state) {
        speed = state == State.Chase ? chaseSpeed
                                     : roamSpeed;
        this.state = state;
    }

    //Deals damage to the player
    public void DealDamage(Player player) {
        if (Time.time < damageCDEndTime) return;
        player.TryDamage(1);
        damageCDEndTime = Time.time + damageCD; //Store the time bat dealt damage
        ApplySelfKnockback();
    }

    //Applies knockback to the bat and disables its attacks
    private void ApplySelfKnockback() {
        Vector3 kbDir = (transform.position - player.transform.position).normalized;
        kbDir.y = 0;

        TryLongPush(kbDir, knockbackForce, knockbackDuration);

        isKnockedBack = true;
        attackTrigger.SetCanAttack(false); //Make sure the scaramite can't attack while being knocked back

        knockbackEndTime = damageCDEndTime + knockbackDuration; //Store the time when knockback ends
    }


    //Roam mode will have the scaramite move randomly within its vision
    private void Roam() {

        timer += FixedDeltaTime;

        if (Vector3.Distance(destination, transform.position) < stoppingDistance) { //If scaramite is about to reach destination
            DoRotation();
        } else {
            Vector3 dir = (destination - transform.position).normalized;
            Vector3 moveVector = roamSpeed * dir;
            controller.Move(moveVector * FixedDeltaTime);
        }

        if (timer >= timeBetweenMove) {
            destination = GetRoamDestination(); //Get new destination
            timer = 0f;
        }
    }


    //Chase mode will have the bat move faster and constantly directly towards the player
    private void Chase() {
        Vector3 playerPos = new (player.transform.position.x, 0,
                                 player.transform.position.z);

        Vector3 moveVector = (playerPos - transform.position) * chaseSpeed;
        controller.Move(moveVector * FixedDeltaTime);
    }

    //Returns a random position within the bounds of the bat's vision
    private Vector3 GetRoamDestination() {
        float distance = Random.Range(minRoamDistance, maxRoamDistance);
        bool pathIsValid = PathfindingUtils.FindRandomRoamingPoint(transform.position, distance,
                                                                   10, out Vector3 clearPoint);
        Debug.Log($"Destination: {clearPoint}, Transform: {transform.position}");
        return pathIsValid ? clearPoint : transform.position;
    }

    //Rotates the bat 75% of the way towards the player
    private void DoRotation() {
        Vector3 dir = transform.position - player.transform.position;

        dir.y = 0; //Don't rotate on y-axis

        //Make sure the bat isn't going 100% towards the player 
        //(it's actually unplayable otherwise lol)
        dir.x *= playerBias;
        dir.z *= playerBias;

        if (dir != Vector3.zero) {
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                                                  Quaternion.LookRotation(dir),
                                                  DeltaTime * angularSpeed);
        }
    }

    public override void Perish() {
        base.Perish();
        DetachModules();
        enabled = false;
        //attackTrigger.Disable();
        visionCollider.enabled = false;
        //rb.constraints = new();
        //rb.isKinematic = false;
        //rb.useGravity = true;
        Destroy(gameObject, 2);
    }
}