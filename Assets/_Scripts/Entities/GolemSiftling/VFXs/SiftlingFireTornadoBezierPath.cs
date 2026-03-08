using System.Collections;
using UnityEngine;
using UnityEngine.Splines;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SiftlingFireTornadoBezierPath : MonoBehaviour
{
    /// <summary>
    /// Fire when the tornado finished its forward path;
    /// </summary>
    public event System.Action OnPathEndpoint;
    /// <summary>
    /// Fired when the tornado fully returns;
    /// </summary>
    public event System.Action OnPathComplete;

    [SerializeField] private GolemSiftling siftling;
    [SerializeField] private Transform fireTornado;
    [SerializeField] private AnimationCurve pathingCurve;
    [SerializeField] private Transform pathStart, pathT1, pathT2, pathEnd, pathEndpoint;

    private BezierCurve bezierPath;
    private float totalPathLength, bezierPathPercent;

    private Vector3 pathStartLocal, pathStartLocalFlipped,
                    pathT1Local, pathT1LocalFlipped;
    private float lerpVal;

    private enum State { Idle, PathingIn, PathingOut }
    private State state;

    void Awake() {
        pathStartLocal = pathStart.localPosition;
        pathStartLocalFlipped = Vector3.Scale(pathStartLocal, new(-1, 1, 1));
        pathT1Local = pathT1.localPosition;
        pathT1LocalFlipped = Vector3.Scale(pathT1Local, new(-1, 1, 1));

        BuildPath(siftling.IsTornadoFlipped);

        float bezierPathLength = CurveUtility.ApproximateLength(bezierPath);
        float endpointLength = Vector3.Distance(pathEnd.position, pathEndpoint.position);
        totalPathLength = bezierPathLength + endpointLength;
        bezierPathPercent = bezierPathLength.SafeDivide(totalPathLength, 0);
    }

    private void BuildPath(bool isFlipped) {
        pathStart.localPosition = isFlipped ? pathStartLocalFlipped : pathStartLocal;
        pathT1.localPosition = isFlipped ? pathT1LocalFlipped : pathT1Local;
        bezierPath = new(pathStart.position, pathT1.position, pathT2.position, pathEnd.position);
    }

    public void Play(float rotationSpeed) {
        state = State.PathingIn;

        BuildPath(siftling.IsTornadoFlipped);
        lerpVal = 0;

        float orbitalSpeed = rotationSpeed * Mathf.Deg2Rad * Vector3.Distance(fireTornado.transform.position, transform.position);
        float rawLerpRate = orbitalSpeed.SafeDivide(totalPathLength);

        StopAllCoroutines();
        StartCoroutine(IDoPathMove(rotationSpeed, rawLerpRate));
    }

    private IEnumerator IDoPathMove(float rotationSpeed, float rawLerpRate) {

        SetState(State.PathingIn, out float target);

        Vector3 siftlingCenter = new(siftling.transform.position.x,
                                     fireTornado.position.y,
                                     siftling.transform.position.z);
        float radius = Vector3.Distance(fireTornado.position, siftlingCenter);
        Vector3 startDirection = (fireTornado.position - siftlingCenter).normalized;
        Vector3 endDirection = (pathStart.position - siftlingCenter).normalized;

        float signedAngle = Vector3.SignedAngle(startDirection, endDirection, Vector3.up);
        float directionMult = siftling.TornadoDirectionMultiplier;

        if (directionMult > 0 && signedAngle < 0) signedAngle += 360f; /// Keep angle positive if counter-clockwise;
        if (directionMult < 0 && signedAngle > 0) signedAngle -= 360f; /// Keep angle negative if clock-wise;

        float duration = Mathf.Abs(signedAngle) / rotationSpeed;
        float timer = 0f;
        while (timer < duration) {
            fireTornado.position = siftlingCenter + Quaternion.AngleAxis(signedAngle * timer / duration, Vector3.up) * startDirection * radius;
            timer = Mathf.MoveTowards(timer, duration, Time.deltaTime);
            yield return null;
        }

        while (state != State.Idle) {
            AdvancePath(rawLerpRate, target);

            if (lerpVal == target) {
                switch (state) {
                    case State.PathingIn:
                        OnPathEndpoint?.Invoke();
                        SetState(State.PathingOut, out target);
                        break;
                    case State.PathingOut:
                        SetState(State.Idle, out _);
                        break;
                }
            }

            yield return null;
        }

        OnPathComplete?.Invoke();

        siftling.FlipTornadoDirection();
    }

    public float GetPathStartAngle(bool isFlipped) {
        Vector3 direction = isFlipped ? pathStartLocalFlipped : pathStartLocal;
        direction.y = 0;
        return Vector3.SignedAngle(Vector3.right, direction.normalized, Vector3.up);
    }
    
    private void SetState(State state, out float target) {
        this.state = state;
        target = state switch { State.PathingIn => 1, _ => 0 };
    }

    private void AdvancePath(float rawLerpRate, float target) {
        lerpVal = Mathf.MoveTowards(lerpVal, target, Time.deltaTime * rawLerpRate * pathingCurve.Evaluate(lerpVal));
        fireTornado.position = EvaluatePath(lerpVal);
    }

    private Vector3 EvaluatePath(float lerpVal) {
        return lerpVal < bezierPathPercent ? CurveUtility.EvaluatePosition(bezierPath, lerpVal.SafeDivide(bezierPathPercent, 0))
                                           : Vector3.Lerp(pathEnd.position, pathEndpoint.position, (lerpVal - bezierPathPercent) / (1 - bezierPathPercent));
    }

    #if UNITY_EDITOR
    void OnDrawGizmos() {
        if (fireTornado) {
            using (new Handles.DrawingScope(Color.red)) {
                Handles.DrawWireDisc(transform.position, Vector3.up, Vector3.Distance(transform.position, fireTornado.position));
                if (pathStart && pathT1 && pathT2 && pathEnd) {
                    Handles.DrawBezier(pathStart.position, pathEnd.position, pathT1.position, pathT2.position, Handles.color, EditorGUIUtility.whiteTexture, 1);
                    if (pathEndpoint) {
                        Handles.DrawLine(pathEnd.position, pathEndpoint.position);
                    }
                }
            }
        }
    }
    #endif
}
