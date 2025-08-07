using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassInstance : MonoBehaviour {
    [SerializeField] private Vector2 scaleRange;
    [SerializeField] private bool cluster;
    [SerializeField] private int clusterCount;
    [SerializeField] private float radius;
    [SerializeField] private GameObject grass;
    void Start() {
        transform.localScale =
            new Vector3(Random.Range(scaleRange.x, scaleRange.y), Random.Range(scaleRange.x, scaleRange.y), Random.Range(scaleRange.x, scaleRange.y));
        
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.y = Random.Range(0f, 360f);
        transform.eulerAngles = currentRotation;

        if (!cluster || !grass) return;
        for (int i = 0; i < clusterCount; i++)
        {
            // Generate a random position around the current object
            Vector3 randomPosition = RandPos();

            // Instantiate the prefab at the random position
            GameObject grassInst = Instantiate(grass, randomPosition, Quaternion.identity);
            grassInst.transform.localScale = new Vector3(Random.Range(scaleRange.x, scaleRange.y),
                Random.Range(scaleRange.x, scaleRange.y), Random.Range(scaleRange.x, scaleRange.y));
        }
    }

    private Vector3 RandPos() {
        float angle = Random.Range(0f, 360f);
        float distance = Random.Range(0f, radius);
        float xOffset = Mathf.Cos(angle * Mathf.Deg2Rad) * distance;
        float zOffset = Mathf.Sin(angle * Mathf.Deg2Rad) * distance;

        // Preserve the Y position of the current object
        return new Vector3(transform.position.x + xOffset, transform.position.y, transform.position.z + zOffset);
    }
}
