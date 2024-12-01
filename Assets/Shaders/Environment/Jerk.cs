using UnityEngine;
using System.Collections;

using System.Collections;
using UnityEngine;

public class Jerk : MonoBehaviour
{
    // Adjustable properties
    public float movementDistance = 1f; // Maximum distance to move
    public float movementDuration = 1f; // How long the movement lasts
    public float returnDuration = 1f;   // How long the return lasts
    public float interval = 2f;         // Time between movements

    private Vector3 originalPosition;
    private bool isMoving = false;

    void Start()
    {
        // Store the original position
        originalPosition = transform.position;
        // Start the movement coroutine
        StartCoroutine(MoveRandomly());
    }

    IEnumerator MoveRandomly()
    {
        while (true)
        {
            if (!isMoving)
            {
                isMoving = true;

                // Generate a random direction
                Vector3 randomDirection = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                ).normalized;

                // Calculate the target position
                Vector3 targetPosition = originalPosition + randomDirection * movementDistance;

                // Move to the target position
                yield return StartCoroutine(TranslateObject(transform.position, targetPosition, movementDuration));

                // Return to the original position
                yield return StartCoroutine(TranslateObject(transform.position, originalPosition, returnDuration));

                isMoving = false;
            }

            // Wait for the interval before the next movement
            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator TranslateObject(Vector3 from, Vector3 to, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the object reaches the exact target position
        transform.position = to;
    }
}