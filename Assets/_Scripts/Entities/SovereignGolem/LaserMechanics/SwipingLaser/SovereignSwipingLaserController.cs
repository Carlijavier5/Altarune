using UnityEngine;

public class SovereignSwipingLaserController : MonoBehaviour {

    public event System.Action<SovereignSwipingLaserController> OnSwipeEnd;

    [Header("Swiping Laser Values")]
    [SerializeField] private Transform laserRoot;
    [SerializeField] private SovereignLaserWarning warning;
    [SerializeField] private SovereignSwipingLaser swipingLaser;
    [SerializeField] private Transform leftEndpoint, rightEndpoint;

    void Awake() {
        swipingLaser.OnSwipeEnd += SwipingLaser_OnSwipeEnd;
    }

    public void DoLaserSwipe(SwipeDirection swipeDirection, float warningTime, float swipeTime) {
        Transform source = swipeDirection == SwipeDirection.LeftRight ? leftEndpoint : rightEndpoint;
        Transform target = swipeDirection == SwipeDirection.LeftRight ? rightEndpoint : leftEndpoint;

        Vector3 sourceDir = source.position - transform.position;
        Quaternion sourceRotation = Quaternion.LookRotation(sourceDir, Vector3.up);
        Vector3 targetDir = target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);

        laserRoot.rotation = sourceRotation;

        warning.DoWarning(warningTime);
        warning.OnWarningFinished += () => {
            swipingLaser.DoTurnPath(sourceRotation, targetRotation, swipeTime);
        };
    }

    private void SwipingLaser_OnSwipeEnd() => OnSwipeEnd?.Invoke(this);
}