using System;
using DG.Tweening;
using UnityEngine;

public class CommonEnemyHealthBar : MonoBehaviour
{
    private Vector3 initialPosition;
    private int damage;
    private float fillAmount;
    private bool shakeEffect = false;
    private float time;
    private float interpTime;
    private float topTime;
    private float originalInterpFill;
    private float originalTopFill;

    [SerializeField] private UnityEngine.UI.Image topLayer;
    [SerializeField] private UnityEngine.UI.Image interpolationLayer;
    [SerializeField] private UnityEngine.UI.Image backgroundLayer;
    [SerializeField] private Transform anchor;
    [SerializeField] private BaseObject attachedEntity; //Change to damagable when using on Enemies. Set to TestCanvas for testing
    [SerializeField] private Damageable damageable;
    int maxHealth;
    void Awake(){
        attachedEntity.OnHealingReceived += OnHealRecieved;
        attachedEntity.OnDamageReceived += OnDamageTaken;
        damageable.OnModuleInit += OnEntityInit;
        fillAmount = 1;
    }


    void Update()
    {
        lockRotation();
        updateInterpolationLayer();
        updateTopLayer();
        shaking();
    }

    private void OnEntityInit(){
        maxHealth = damageable.Health;
    }
    private void lockRotation(){
        Camera camera = Camera.main;
        //Vector3 layerRotation = Quaternion.LookRotation(camera.transform.position).eulerAngles
        //layerRotation.y = layerRotation.z = 0;
        //this.transform.rotation = Quaternion.Euler(layerRotation);
        //anchor.transform.localRotation = Quaternion.Euler(layerRotation)
        Vector3 direction = (anchor.transform.position - Camera.main.transform.position).normalized;
        direction.x = 0;
        Quaternion lookTarget = Quaternion.LookRotation(direction, Vector3.up);
        anchor.transform.rotation = lookTarget;
        //anchor.LookAt(camera.transform);
        //topLayer.transform.rotation = Quaternion.Euler(layerRotation);
        //backgroundLayer.transform.rotation = Quaternion.Euler(layerRotation);
        //interpolationLayer.transform.rotation = Quaternion.Euler(layerRotation);
    }

    private void OnHealRecieved(int heal){
        fillAmount = (float) attachedEntity.Health / maxHealth;
        originalTopFill = topLayer.fillAmount;
    }

    private void updateInterpolationLayer(){
        if(interpolationLayer.fillAmount > fillAmount) {
            interpTime += Time.deltaTime;
            interpolationLayer.fillAmount = Mathf.Lerp(originalInterpFill, fillAmount, interpTime);
            if(interpTime >= 1){
                interpTime = 0;
            }
        } else if(interpolationLayer.fillAmount < fillAmount){
                interpolationLayer.fillAmount = fillAmount;
        }
    }

    private void updateTopLayer(){
        if(topLayer.fillAmount > fillAmount){
            topLayer.fillAmount = fillAmount;
        } else if(topLayer.fillAmount < fillAmount){
            topTime += Time.deltaTime;
            topLayer.fillAmount = Mathf.Lerp(originalTopFill, fillAmount, topTime);
            if(topTime >= 1){
                topTime = 0;
            }
        }
        if(fillAmount == 0){
            Destroy(gameObject);
        }
    } 

    private void shaking(){
        if(shakeEffect){
            if(time == 0){
                initialPosition = transform.localPosition;
            }
            time += Time.deltaTime;
            float shakeAmount = 0.1f * (float)damage / 4;
            float shakeSpeed = 50f * (float)damage / 4;
            if(shakeSpeed < 30f){
                shakeSpeed = 30f;
            }
            if(shakeAmount < 0.07f){
                shakeAmount = 0.07f;
            }
            gameObject.transform.localPosition = initialPosition + new Vector3(Mathf.Sin(Time.time * shakeSpeed) * shakeAmount,0,Mathf.Sin(Time.time * shakeSpeed) * shakeAmount);
            if(time >= 0.5){
                time = 0;
                shakeEffect = false;
                gameObject.transform.localPosition = initialPosition;
            }
        }
    }

    private void OnDamageTaken(int damage){
        originalInterpFill = interpolationLayer.fillAmount;
        fillAmount = (float) attachedEntity.Health / maxHealth;
        shakeEffect = true;
        time = 0;
        this.damage = damage;
    }
}
