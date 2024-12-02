using System.Collections;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour {

    [SerializeField] private Meteor[] meteors;
    [SerializeField] private Vector2 fieldDimensionsX,
                                     fieldDimensionsZ,
                                     meteorSizeRange;

    private Vector3 RandomSpawnPosition {
        get {
            float positionX = Random.Range(fieldDimensionsX.x,
                               fieldDimensionsX.y),
                  positionZ = Random.Range(fieldDimensionsZ.x,
                                           fieldDimensionsZ.y);
            return new Vector3(positionX, 0, positionZ);
        }
    }

    private Vector3 RandomSize {
        get {
            return Random.Range(meteorSizeRange.x,
                                meteorSizeRange.y) * Vector3.one;
        }
    }

    void Awake() {
        transform.SetParent(null);
    }

    public void DoMeteorHurl(int amount, float intervalTime,
                             float riseDuration, float fallDuration) {
        StartCoroutine(IDoMeteorHurl(amount, intervalTime,
                                     riseDuration, fallDuration));
    }

    private IEnumerator IDoMeteorHurl(int amount, float intervalTime,
                                      float riseDuration, float fallDuration) {
        amount = Mathf.Clamp(amount, 1, meteors.Length);
        float duration = riseDuration / amount;
        for (int i = 0; i < amount; i++) {
            meteors[i].DoRise(RandomSpawnPosition, RandomSize, duration);
            yield return new WaitForSeconds(intervalTime);
        }

        float leftOverTime = riseDuration - duration - intervalTime * (amount - 1);
        yield return new WaitForSeconds(Mathf.Max(0, leftOverTime));

        duration = fallDuration / amount;
        for (int i = 0; i < amount; i++) {
            meteors[i].DoFall(duration);
            yield return new WaitForSeconds(intervalTime);
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Vector3 center = new((fieldDimensionsX.x + fieldDimensionsX.y) / 2,
                             transform.position.y,
                             (fieldDimensionsZ.x + fieldDimensionsZ.y) / 2);
        Vector3 size = new(fieldDimensionsX.y - fieldDimensionsX.x,
                           transform.position.y,
                           fieldDimensionsZ.y - fieldDimensionsZ.x);
        Gizmos.DrawWireCube(center, size);
        Gizmos.color = Color.white;
    }
    #endif
}