using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleWindEffect : MonoBehaviour
{
    //Placeholder WindTower visual effects

    [SerializeField] private float growSpeed;

    private Vector3 targetScale;
 
    void Awake(){
        targetScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }
    void Update(){
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, growSpeed * Time.deltaTime);
        if (transform.localScale == targetScale) Destroy(gameObject);
    }
}
