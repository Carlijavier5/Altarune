using System.Linq;
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SiftlingGroundCrackController : MonoBehaviour
{
    private readonly int GROUND_FILL_PARAM = Shader.PropertyToID("_Fill");

    [SerializeField] private SiftlingFireTornadoBezierPath bezierPath;
    [SerializeField] private Renderer emberTrail, shadowTrail;
    [Tooltip("Position alongside the bezier path where the trails begin to spawn;")]
    [SerializeField] private float crackStartLerp;

    [System.Serializable]
    private class GroundCrackStep {
        public float triggerLerpVal;
        public ParticleSystem[] associatedVFXs;

        public void Play() {
            foreach (ParticleSystem vfx in associatedVFXs) {
                vfx.Play();
            }
        }
    }
    [Tooltip("Positions along the path where VFXs are spawned;")]
    [SerializeField] private GroundCrackStep[] steps;
    private int groundCrackStepIndex;

    [Tooltip("How long does each trail stay up before the fadeout starts, and after the forward path ends;")]
    [SerializeField] private float emberTrailDuration, shadowTrailDuration;
    [Tooltip("How long does the trail fadeout last;")]
    [SerializeField] private float trailFadeDuration;

    [SerializeField] private ParticleSystem[] smokePool;
    [SerializeField] private Transform[] potentialSmokeSpawn;
    [Tooltip("Range of smoke VFXs that may spawn, should be smaller than the pool length;")]
    [SerializeField] private Vector2Int smokeSpawnAmountRange;
    [Tooltip("How long after the forward path ends do smokes begin to spawn")]
    [SerializeField] private float smokeSpawnDelay;
    [Tooltip("Interval in-between the spawning of smoke VFXs")]
    [SerializeField] private Vector2 smokeSpawnIntervalRange;
    private int smokeVFXIndex;

    private MaterialPropertyBlock emberMPB, shadowMPB;

    void Awake() {
        emberMPB = new();
        shadowMPB = new();

        emberTrail.GetPropertyBlock(emberMPB);
        shadowTrail.GetPropertyBlock(shadowMPB);
        SetTrailFill(emberMPB, emberTrail, 0);
        SetTrailFill(shadowMPB, shadowTrail, 0);

        bezierPath.OnPathStart += BezierPath_OnPathStart;
    }

    private void BezierPath_OnPathStart() {
        groundCrackStepIndex = 0;
        emberTrail.GetPropertyBlock(emberMPB);
        shadowTrail.GetPropertyBlock(shadowMPB);

        bezierPath.OnPathAdvance += BezierPath_OnPathAdvance;
        bezierPath.OnPathEndpoint += BezierPath_OnPathEndpoint;
    }

    private void BezierPath_OnPathAdvance(float lerpVal) {

        if (lerpVal > crackStartLerp) {
            float scaledLerp = (lerpVal - crackStartLerp) / (1 - crackStartLerp);
            SetTrailFill(emberMPB, emberTrail, scaledLerp);
            SetTrailFill(shadowMPB, shadowTrail, scaledLerp);
        }

        if (groundCrackStepIndex < steps.Length
                && lerpVal > steps[groundCrackStepIndex].triggerLerpVal) {

            steps[groundCrackStepIndex].Play();
            groundCrackStepIndex++;
        }
    }

    private void BezierPath_OnPathEndpoint() {
        bezierPath.OnPathAdvance -= BezierPath_OnPathAdvance;
        bezierPath.OnPathEndpoint -= BezierPath_OnPathEndpoint;
        StopAllCoroutines();
        StartCoroutine(IDoSmokeSpawn());
        StartCoroutine(IDoTrailFade(emberMPB, emberTrail, emberTrailDuration));
        StartCoroutine(IDoTrailFade(shadowMPB, shadowTrail, shadowTrailDuration, true));
    }

    private void SetTrailFill(MaterialPropertyBlock mpb, Renderer renderer, float fillValue) {
        mpb.SetFloat(GROUND_FILL_PARAM, fillValue);
        renderer.SetPropertyBlock(mpb);
    }

    private void PlaceSmoke(Transform smokeSpawn) {
        ParticleSystem smokeVFX = smokePool[smokeVFXIndex];
        smokeVFX.transform.position = smokeSpawn.position;
        smokeVFX.Play();
        smokeVFXIndex = (smokeVFXIndex + 1) % smokePool.Length;
    }

    private IEnumerator IDoSmokeSpawn() {

        int smokeSpawnAmount = Random.Range(smokeSpawnAmountRange.x, smokeSpawnAmountRange.y);

        System.Random rnGesus = new();
        int[] spawnIndeces = Enumerable.Range(0, potentialSmokeSpawn.Length)
                                       .OrderBy(_ => rnGesus.Next())
                                       .Take(smokeSpawnAmount).ToArray();

        yield return new WaitForSeconds(smokeSpawnDelay);

        int currentIndex = 0;
        float spawnInterval;
        Transform smokeSpawn;
        while (currentIndex < spawnIndeces.Length) {
            spawnInterval = Random.Range(smokeSpawnIntervalRange.x, smokeSpawnIntervalRange.y);
            yield return new WaitForSeconds(spawnInterval);

            smokeSpawn = potentialSmokeSpawn[spawnIndeces[currentIndex]];
            PlaceSmoke(smokeSpawn);
            currentIndex++;
        }
    }

    private IEnumerator IDoTrailFade(MaterialPropertyBlock mpb, Renderer renderer, float duration, bool fadeSmokes = false) {
        yield return new WaitForSeconds(duration);

        if (fadeSmokes) {
            foreach (ParticleSystem smokeVFX in smokePool) {
                //smokeVFX.Stop();
            }
        }

        renderer.GetPropertyBlock(mpb);
        float lerpVal = mpb.GetFloat(GROUND_FILL_PARAM);

        while (lerpVal > 0) {
            lerpVal = Mathf.MoveTowards(lerpVal, 0, Time.deltaTime.SafeDivide(duration));
            SetTrailFill(mpb, renderer, lerpVal);
            yield return null;
        }
    }

    #if UNITY_EDITOR
    private const float EDITOR_GIZMOS_DISC_RADIUS = 0.1f;

    private void OnDrawGizmosSelected() {
        if (bezierPath && steps != null) {
            
            float[] lerpVals = new float[steps.Length + 1];
            lerpVals[0] = crackStartLerp;

            for (int i = 1; i < lerpVals.Length; i++) {
                lerpVals[i] = steps[i - 1].triggerLerpVal;
            }

            Vector3[] points = bezierPath.Editor_EvaluatePath(lerpVals);
            using (new Handles.DrawingScope(Color.green)) {
                Handles.DrawWireDisc(points[0], Vector3.up, EDITOR_GIZMOS_DISC_RADIUS);
            }
            using (new Handles.DrawingScope(Color.blue)) {
                for (int i = 1; i < points.Length; i++) {
                    Handles.DrawWireDisc(points[i], Vector3.up, EDITOR_GIZMOS_DISC_RADIUS);
                }
            }
        }
    }
    #endif
}
