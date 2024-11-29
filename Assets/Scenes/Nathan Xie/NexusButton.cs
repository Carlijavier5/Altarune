using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NexusButton : MonoBehaviour
{
    private Vector3 destination;
    private Vector3 start;
    private Vector3 currentPos;
    // Update is called once per frame
    void Update()
    {
        Move();
    }
    public void Move(){
        if(currentPos.x != destination.x) {
            currentPos.x = Mathf.MoveTowards(currentPos.x, destination
            .x, Time.deltaTime * 15);
        } else if (currentPos.y != destination.y) {
            currentPos.y = Mathf.MoveTowards(currentPos.y, destination.y, Time.deltaTime * 15);
        }
        transform.localPosition = currentPos;
    }
    public void StartMovement(){
        this.transform.localPosition = start;
        currentPos = transform.localPosition;
    }
    public void SetDestination(Vector3 destination){
        currentPos = transform.localPosition;
        this.destination = destination;
    }
    public void SetStart(Vector3 start){
        this.start = start;
    }
}
