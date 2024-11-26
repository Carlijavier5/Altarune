using System.Collections.Generic;
using UnityEngine;

public class SovereignSwipingLaserMaster : SovereignPhaseMaster<SwipingLaserProperties> {

    public event System.Action OnAttackEnd;

    [SerializeField] private SovereignSwipingLaserController[] laserControllers;

    private readonly HashSet<SovereignSwipingLaserController> activeLasers = new();
    private SwipeDirection prevDirection;
    private int laserCounter;

    protected override void Awake() {
        base.Awake();
        foreach (SovereignSwipingLaserController controller in laserControllers) {
            controller.OnSwipeEnd += Controller_OnSwipeEnd;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Y)) {
            EnterPhase(SovereignPhase.Macro2);
            DoAttack();
        }
        if (Input.GetKeyDown(KeyCode.U)) {
            EnterPhase(SovereignPhase.Macro3);
            DoAttack();
        }
        if (Input.GetKeyDown(KeyCode.I)) {
            EnterPhase(SovereignPhase.Macro4);
            DoAttack();
        }
    }

    public void DoAttack() {
        laserCounter = activeConfig.laserCount;
        DoSwipeLaser();
    }

    private void DoSwipeLaser() {
        laserCounter--;
        prevDirection = prevDirection == SwipeDirection.LeftRight ? SwipeDirection.RightLeft
                                                                  : SwipeDirection.LeftRight;
        foreach (SovereignSwipingLaserController controller in laserControllers) {
            if (!activeLasers.Contains(controller)) {
                activeLasers.Add(controller);
                controller.DoLaserSwipe(prevDirection, activeConfig.warningTime, activeConfig.swipeTime);
                break;
            }
        }
    }

    private void Controller_OnSwipeEnd(SovereignSwipingLaserController laser) {
        activeLasers.Remove(laser);
        if (laserCounter > 0) DoSwipeLaser();
        else {
            OnAttackEnd?.Invoke();
            OnAttackEnd = null;
        }
    }
}