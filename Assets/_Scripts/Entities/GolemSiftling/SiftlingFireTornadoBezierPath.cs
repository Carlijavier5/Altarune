using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SiftlingFireTornadoBezierPath : MonoBehaviour
{
    [SerializeField] private Transform fireTornado;
    [SerializeField] private AnimationCurve pathingCurve;
    [SerializeField] private Transform pathStart, pathT1, pathT2, pathEnd, pathEndpoint;

    private BezierCurve bezierPath;
    private float lerpVal;
    private bool isPathFlipped;

    private enum State { Idle, PathingIn, PathingOut }
    private State state;

    void Awake() {
        BuildPath(isPathFlipped);
    }

    private void BuildPath(bool isFlipped) {
        Vector3 controlPointFlip = isFlipped ? new Vector3(-1, 1, 1) : Vector3.one;
        bezierPath = new(Vector3.Dot(pathStart.position, controlPointFlip),
                         Vector3.Dot(pathT1.position, controlPointFlip),
                         Vector3.Dot(pathT2.position, controlPointFlip),
                         Vector3.Dot(pathEnd.position, controlPointFlip));
    }

    public void Play(float rotationSpeed) {
        state = State.Idle;
        StopAllCoroutines();
        StartCoroutine(IDoPathMove(rotationSpeed));
    }

    private IEnumerator IDoPathMove(float rotationSpeed) {
        while (state != State.Idle) {
            lerpVal = Mathf.MoveTowards(lerpVal, state switch { State.PathingIn => 1, _ => 0 }, Time.deltaTime);
            fireTornado.position = CurveUtility.EvaluatePosition(bezierPath, pathingCurve.Evaluate(lerpVal));
            yield return null;
        }
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
