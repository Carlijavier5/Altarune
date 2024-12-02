using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GatlingGun : Summon {
    [SerializeField] private AggroRange aggroRange;
    //[SerializeField] private Transform rootTransform;
    [SerializeField] private SmallGatlingArea smallArea;
    [SerializeField] private BigGatlingArea bigArea;
    //[SerializeField] private float rotationSpeed = 5f;
    [Header("Timers")]
    [SerializeField] private float bigAreaDuration = 10f;
    [SerializeField] private float smallAreaDuration = 3f;
    [SerializeField] private float smallAreaSpawnDelay = 0.5f;
    [SerializeField] private float bigAreaSpawnDelay = 3.0f;
    [Header("Sizes")]
    [SerializeField] private float bigAreaSize = 5.0f;
    [SerializeField] private float minSmallSize = 0.5f;
    [SerializeField] private float maxSmallSize = 2.0f;
    [SerializeField] private int maxSmallAreas = 3;
    [Header("Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageInterval = 1f;
    [Header("Visual")] [SerializeField] private GatlingAnimator animator;

    private Entity aggroTarget;
    private BigGatlingArea currentBigArea;
    public List<SmallGatlingArea> SmallAreas { get; private set; } = new List<SmallGatlingArea>();
    public int NumOfInactive { get; set; }
    private bool isAggroed = false;
    private float smallAreaSpawnTimer = 0f;
    private float bigAreaSpawnTimer = 0f;
    private bool targetSet;
    //private Quaternion targetRotation;

    void Awake() {
        bigAreaSpawnTimer = bigAreaSpawnDelay;
        aggroRange.OnAggroEnter += AggroRange_OnAggroEnter;
        aggroRange.OnAggroExit += AggroRange_OnAggroExit;
    }

    void Update() {
        // Calls SpawnSmallArea on a delay
        if (active && isAggroed && currentBigArea != null &&
           (SmallAreas.Count < maxSmallAreas || NumOfInactive >= 1)) {
            smallAreaSpawnTimer += Time.deltaTime;
            if (smallAreaSpawnTimer >= smallAreaSpawnDelay) {
                SpawnSmallArea();
                smallAreaSpawnTimer = 0f;
            }
        }
        // Calls SpawnBigArea on a delay
        if (active && currentBigArea == null) {
            bigAreaSpawnTimer += Time.deltaTime;
            if (bigAreaSpawnTimer >= bigAreaSpawnDelay && aggroTarget != null) {
                SpawnBigArea();
                isAggroed = true;
                bigAreaSpawnTimer = 0f;
            }
        }
    }

    private void AggroRange_OnAggroEnter(Entity _) => UpdateAggro();

    private void AggroRange_OnAggroExit(Entity _) => UpdateAggro();

    private void UpdateAggro() {
        if (aggroRange.AggroTargets.Count > 0) {
            Entity closestTarget = aggroRange.AggroTargets.First();
            float closestDistance = Vector3.Distance(closestTarget.transform.position,
                                                     transform.position);
            foreach (Entity target in aggroRange.AggroTargets) {
                float newDistance = Vector3.Distance(target.transform.position,
                                                     transform.position);
                if (newDistance < closestDistance) {
                    closestTarget = target;
                    closestDistance = newDistance;
                }
            }
            aggroTarget = closestTarget;
        } else {
            aggroTarget = null;
        }
    }

    /// <summary>
    /// Destroys all areas and lets new big area spawn
    /// </summary>
    public void StopAggro() {
        isAggroed = false;
        if (currentBigArea != null) Destroy(currentBigArea.gameObject);
        SmallAreas.ForEach(area => Destroy(area.gameObject));
        SmallAreas.Clear();
        NumOfInactive = 0;
    }

    /// <summary>
    /// Spawns a large area
    /// </summary>
    private void SpawnBigArea() {
        if (currentBigArea != null) Destroy(currentBigArea);
        Vector3 areaPosition = aggroTarget.transform.position;
        currentBigArea = Instantiate(bigArea, areaPosition, Quaternion.identity);
        animator.SemiFire(areaPosition, bigAreaDuration);
        currentBigArea.GetComponent<BigGatlingArea>().Init(bigAreaDuration, bigAreaSize, this);
        // Get rotation towards big area
        //Vector3 direction = (currentBigArea.transform.position - transform.position).normalized;
        //targetRotation = Quaternion.LookRotation(direction);
        //targetRotation *= Quaternion.Euler(0, -90, 0);
        //StartCoroutine(Rotate());
    }

    /// <summary>
    /// Spawns a small area
    /// </summary>
    private void SpawnSmallArea() {
        // Create a random position within large radius - small radius
        Vector2 randomPos = Random.insideUnitCircle * (currentBigArea.transform.localScale.x / 2f - maxSmallSize / 2.5f);
        Vector3 smallAreaPos = currentBigArea.transform.position + new Vector3(randomPos.x, 0, randomPos.y);
        if (SmallAreas.Count < maxSmallAreas) {
            // Create more small areas if they don't exist.
            SmallGatlingArea newSmallArea = Instantiate(smallArea, smallAreaPos, Quaternion.identity);
            newSmallArea.Init(smallAreaDuration, damage, damageInterval, minSmallSize, maxSmallSize, this);
            SmallAreas.Add(newSmallArea);
        } else {
            // Enable existing areas
            for (int i = 0; i < SmallAreas.Count; i++) {
                if (!SmallAreas[i].gameObject.activeSelf) {
                    SmallAreas[i].transform.position = smallAreaPos;
                    SmallAreas[i].gameObject.SetActive(true);
                    NumOfInactive--;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Rotate the gatling gun towards a target
    /// </summary>
    /// <returns></returns>
    // private IEnumerator Rotate() {
    //     while (rootTransform.rotation != targetRotation) {
    //         rootTransform.rotation = Quaternion.Slerp(rootTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    //         yield return null;
    //     }
    // }
}
