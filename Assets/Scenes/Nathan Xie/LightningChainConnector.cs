using System;
using UnityEngine;

public class LightningChainConnector : MonoBehaviour
{
    [SerializeField] private Renderer draw;
    [SerializeField] private CapsuleCollider CC;
    private LightningChainBall dominantBall;
    private LightningChainBall submissiveBall;
    public void setAngle(Vector3 start, Vector3 end){
        var v3 = transform.localScale;
        v3.y = Vector3.Magnitude(end - start) / 2f;
        transform.localScale = v3;
        transform.up = end - start;
    }
    public void setParents(LightningChainBall dominant, LightningChainBall submissive) {
        dominant.OnTimeOut += End;
        dominantBall = dominant;
        submissive.OnTimeOut += End;
        submissiveBall = submissive;
    }
    void End(LightningChainBall ball) {
        if(gameObject != null) {
            Destroy(gameObject);
            submissiveBall.OnTimeOut -= End;
            dominantBall.OnTimeOut -= End;
        }
    }
    public void setCollisionStatus(Boolean status) {
        CC.enabled = status;
    }
    public void Flash(Boolean flash){
        draw.enabled = flash;
    }
    void OnTriggerEnter(Collider other){
        if(other.TryGetComponent(out BaseObject baseObject)
        && !(baseObject is Entity && (baseObject as Entity).Faction == EntityFaction.Friendly)){
            baseObject.TryDamage(2);
        }
    }
}
