using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class LightningChainBall : MonoBehaviour
{
    [SerializeField][Tooltip("How long the ball will remain in the game in seconds")] private float aliveTime;
    [SerializeField][Tooltip("The amount of time before the ball becomes active in seconds")] private float startTime;
    [SerializeField][Tooltip("The amount of time the ball spends flashing the line before activating")] private float chargeTime; 
    [SerializeField][Tooltip("The amount of time the ball takes to rise from floor into the air in seconds")] private float riseTime;
    [SerializeField][Tooltip("The prefab for the LightningChainConnector")] private LightningChainConnector connector;
    private float aliveTimer; //Amount of time spent alive
    private float riseTimer; //Amount of time spent rising
    private Vector3 startVector; //Origination vector for parabola
    private Vector3 endVector; //End vector for parabola
    private float airTime; //Amount of time spent in parabolic motion
    private float maxTime = 1f; //Time spent in parabolic motion
    public event System.Action<LightningChainBall> OnTimeOut; //Event for when ball expires
    public event System.Action<LightningChainBall> createdConnection; //Event for when a ball has a connection target
    private HashSet<LightningChainBall> adjacentBalls = new(); //List that stores all balls in range of connection
    private float chargingTime; //The amount of time that has been spent charging
    private LightningChainBall connectedBall; //The Ball which is connected to this ball
    private Boolean activated = false; //Ball has created a active line with another ball
    private Boolean sparkActivated = false; //Ball is eligiable for connections
    private Boolean mutualPartner = false; //Ball has a partner
    private Vector3 floorPosition; //Position of ball when unactivated
    private Vector3 risePosition; //Position of ball when charged, floats in midair

    //If the ball is the dominant in its relationship, 
    //the dominant ball spawns and manages the LightningChainConnector. 
    //The oldest ball is the dominant ball
    private Boolean dominant = false; 
    private LightningChainConnector activeConnector = null; 
    private float fallTimer; //The amount of time spent falling
    private Boolean active = false; //whether the ball is active and should be updating

    private RotateObject rotator;
    private float rotatorSpeed;


    private void Awake() {
        rotator = GetComponentInChildren<RotateObject>();
        rotatorSpeed = rotator.speed;
    }

    // The ball goes through __ stages. It start unactivated and activates after some time. After activating, it begins looking for elgibible connection targets
    //If it finds a target and the connection is mutual, the ball rises into the air
    //After rising into the air, the ball begins sparking by flashing the connector in the air
    //After some time, the ball fully activates the connector and remains in this state until the partner is destroyed or it expires
    void Update()
    {
        if (active) {
            UpdateParbola(); 
            CheckAliveTime();
            CheckConnectionTargets();
            Rise();
            Fall();
            CheckSparkingStatus();
        }

        if (transform.position.y > 0.2f) {
            rotator.speed = rotatorSpeed;
        }
        else {
            rotator.speed = 0f;
        }
    }
    public void End(){
        OnTimeOut?.Invoke(this);
        active = false;
        Destroy(gameObject);
        Destroy(this);
    }
    /// <summary>
    /// Updates the Alive time and deletes the objects if over the time
    /// </summary>
    private void CheckAliveTime(){
        aliveTimer += Time.deltaTime;
        if (aliveTimer >= aliveTime){
            End();
        }
    }
    /// <summary>
    /// updates the parabolic motion for the launch movement. 
    /// </summary>
    private void UpdateParbola(){
        if(airTime < maxTime) {
            airTime += Time.deltaTime;
            transform.position = Parabola(startVector, endVector, 5f, airTime / maxTime);
        }
    }
    /// <summary>
    /// Called after the ball has been alive for startTime seconds.
    /// The method attempts to select a partner for the ball to connect to.
    /// </summary>
    private void CheckConnectionTargets(){
        if (aliveTimer >= startTime &&
        connectedBall == null) {
            //Goes through all balls in range and selects the closest ball in range to connect with.
            sparkActivated = true;
            LightningChainBall tempBall = null;
            foreach (LightningChainBall ball in adjacentBalls) {
                if (tempBall == null && ball.sparkActivated) tempBall = ball;
                else if (tempBall != null  && ball.sparkActivated && 
                Vector3.Magnitude(ball.transform.position - this.transform.position)
                 <= Vector3.Magnitude(tempBall.transform.position - this.transform.position)) tempBall = ball;
            }
            connectedBall = tempBall;
            //A check to ensure that the relationship is mutual
            if(connectedBall != null) { //A ball was selected
                if(connectedBall.aliveTimer >= this.aliveTimer) { //The ball we're connecting with is older than this ball. So, it must have a partner
                    if(System.Object.ReferenceEquals(connectedBall.connectedBall, this)) { //it's partner is this ball.
                        //Relationship is mutual and so we can proceed to the next stage.
                        createdConnection?.Invoke(this);
                        mutualPartner = true;
                        sparkActivated = false;
                        dominant = false;
                        connectedBall.OnTimeOut += DeadPartner;
                        StartCoroutine(CogSpin(0.1f, true));
                    } else {
                        //Relationship is not mutual, this ball will repeat this method
                        mutualPartner = false;
                        sparkActivated = true;
                        connectedBall = null;
                    }
                } else if(connectedBall.aliveTimer < this.aliveTimer){
                    //The ball we are attempting to for a connection with is younger than this ball
                    //Since the ball may not be old enough to formed a connection, we will wait for when that ball forms a connection
                    connectedBall.createdConnection += checkMutualConnection;
                }
            }
        }
    }
    /// <summary>
    /// Checks if the connectionTarget's partner is the this ball or not.
    /// If it is not this ball, this ball removes it as a potential partner,
    /// If it is this ball, this ball coninues to the next stage
    /// </summary>
    /// <param name="connectionTarget">The ball this ball is attempting to form a connection with</param>
    private void checkMutualConnection(LightningChainBall connectionTarget){
        if(System.Object.ReferenceEquals(connectedBall.connectedBall, this)) {
            //The connection is mutual. This ball continues to the next stage.
            createdConnection?.Invoke(this);
            mutualPartner = true;
            sparkActivated = false;
            dominant = true;
            connectedBall.OnTimeOut += DeadPartner;
        } else {
            //The connection is not mutual. This ball goes back to sparking
            mutualPartner = false;
            sparkActivated = true;
            connectedBall = null;
        }
    }
    /// <summary>
    /// Resets the parameters of the ball to the beginning.
    /// </summary>
    private void Clear(){
        fallTimer = 0.0001f;
        riseTimer = 0; 
        chargingTime = 0; 
        connectedBall = null; 
        activated = false; 
        mutualPartner = false; 
        dominant = false;
        activeConnector = null;
    }
    /// <summary>
    /// Called when partner of this ball is despawned. Attatched to the partner's onTimeOut event
    /// </summary>
    /// <param name="ball"></param>
    private void DeadPartner(LightningChainBall ball){
        Clear();
    }

    /// <summary>
    /// Makes the ball rise. Only activates when the ball has a mutual partner and the ball has not risen
    /// </summary>
    private void Rise(){
        if (connectedBall != null && mutualPartner && riseTimer <= riseTime) {
            riseTimer += Time.deltaTime;
            transform.position = Vector3.Lerp(floorPosition, risePosition, riseTimer / riseTime);
        }
    }
    /// <summary>
    /// Makes the ball fall to groundLevel. Only activates when the ball has risen and loses its partner.
    /// </summary>
    private void Fall() {
        if(fallTimer >= 0.0001f && fallTimer <= riseTime){
            fallTimer += Time.deltaTime;
            rotator.speed = 0;
            transform.position = Vector3.Lerp(risePosition, floorPosition, fallTimer / riseTime);
        }
    }
    /// <summary>
    /// Spawns the connector and makes it flash after the ball has risen
    /// </summary>
    private void CheckSparkingStatus(){
        if (connectedBall != null && !activated && riseTimer >= riseTime) {
            Vector3 offset = new Vector3(0, 0.25f / 2f, 0);
            //Spawns the connector only once and if the ball is the dominant one to prevent duplicates
            if (chargingTime == 0 && dominant) {
                activeConnector = Instantiate(connector, (connectedBall.transform.position + offset - this.transform.position + offset) / 2f + this.transform.position + offset, Quaternion.identity);
                activeConnector.setAngle(this.transform.position + offset, connectedBall.transform.position + offset);
                activeConnector.setCollisionStatus(false);
                activeConnector.setParents(this, connectedBall);
            }
            chargingTime += Time.deltaTime;
            //After charging is finished, the connector is activates to deal damage.
            if(chargingTime >= chargeTime) {
                activated = true;
                if(dominant) {
                    activeConnector.setCollisionStatus(true);
                    activeConnector.Flash(true);
                }
            } else if(dominant){
                FlashConnection();
            }
        }
    }
    /// <summary>
    /// Turns the connector's renderer on or off depending on the time to make a flash effect
    /// </summary>
    private void FlashConnection(){
        if(Time.fixedTime % .2 < 0.1)
		{
			activeConnector.Flash(true);
		}
		else{
			activeConnector.Flash(false);
		}
    }
    /// <summary>
    /// Must be called to activate the ball
    /// Set's the start and end positions of the ball.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public void Launch(Vector3 start, Vector3 end) {
        active = true;
        startVector = start + new Vector3(0, 1f, 0);
        endVector = end;
        risePosition = end + new Vector3(0, 1, 0);
        floorPosition = end;
    }
    
    //Ball animator
    private IEnumerator CogSpin(float time, bool enable) {
        yield return new WaitForSeconds(time);
        rotator.speed = enable ? rotatorSpeed : 0;
    }
    /// <summary>
    /// Handles the parabolic motion of the ball
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="height"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    private Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        System.Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }
    void OnTriggerEnter(Collider other){
        if(other.TryGetComponent(out LightningChainBall ball)){
            if(adjacentBalls.Add(ball)){
                ball.OnTimeOut += removeBallFromList;
            }
        }
    }
    void OnTriggerExit(Collider other){
        if(other.TryGetComponent(out LightningChainBall ball)){
            removeBallFromList(ball);
        }
    }
    private void removeBallFromList(LightningChainBall ball){
        adjacentBalls.Remove(ball);
    }
}
