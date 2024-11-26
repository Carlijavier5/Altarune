using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LeftOrRight { Left, Right }

public class PawSlamMaster : SovereignPhaseMaster<PawSlamProperties> {

    public event System.Action OnAttackEnd;
    private const string LEFT_TRIGGER = "SlamLeft",
                         RIGHT_TRIGGER = "SlamRight";

    [SerializeField] private Animator animator;
    [SerializeField] private Transform leftSource, rightSource;
    [SerializeField] private PawSlam[] slams;

    private readonly HashSet<PawSlam> activeSlams = new();
    private LeftOrRight leftOrRight;
    private int leftTrigger, rightTrigger,
                slamCount;

    protected override void Awake() {
        base.Awake();
        foreach (PawSlam slam in slams) {
            slam.OnSmashEnd += Slam_OnSmashEnd;
        }

        leftTrigger = Animator.StringToHash(LEFT_TRIGGER);
        rightTrigger = Animator.StringToHash(RIGHT_TRIGGER);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.G)) {
            EnterPhase(SovereignPhase.Macro2);
            DoAttack();
        }
        if (Input.GetKeyDown(KeyCode.H)) {
            EnterPhase(SovereignPhase.Macro3);
            DoAttack();
        }
        if (Input.GetKeyDown(KeyCode.J)) {
            EnterPhase(SovereignPhase.Macro4);
            DoAttack();
        }
    }

    public void DoAttack() {
        slamCount = activeConfig.slamCount;
        StartCoroutine(IQueueSlams());
    }

    private IEnumerator IQueueSlams() {
        while (slamCount > 0) {
            leftOrRight = leftOrRight == LeftOrRight.Left ? LeftOrRight.Right
                                                          : LeftOrRight.Left;
            animator.SetTrigger(leftOrRight == LeftOrRight.Left ? leftTrigger : rightTrigger);
            slamCount--;
            yield return new WaitForSeconds(activeConfig.timeBetweenSlams);
        }
    }

    public void TrySlam(LeftOrRight leftOrRight) {
        PawSlam availableSlam = slams
                      .FirstOrDefault((slam) => !activeSlams.Contains(slam));
        if (availableSlam) {
            Transform source = leftOrRight == LeftOrRight.Left ? leftSource : rightSource;
            availableSlam.DoSlam(source.position, activeConfig.slamDuration);
            activeSlams.Add(availableSlam);
        }
    }

    private void Slam_OnSmashEnd(PawSlam slam) {
        activeSlams.Remove(slam);
        if (activeSlams.Count == 0
            && slamCount <= 0) OnAttackEnd?.Invoke();
    }
}