using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CommonEnemyHealthBar : MonoBehaviour
{
    Vector3 rotationLock = new Vector3(34.5f, 0, 0);
    int damage;
    float shrinkTime = 1f;
    float fillAmount;
    Boolean shakeEffect = false;
    float time;
    float interpTime;
    float topTime;
    float originalInterpFill;
    float originalTopFill;

    [SerializeField] UnityEngine.UI.Image topLayer;
    [SerializeField] UnityEngine.UI.Image interpolationLayer;
    [SerializeField] UnityEngine.UI.Image backgroundLayer;
    [SerializeField] TestCanvas attachedEntity; //Change to damagable when using on Enemies. Set to TestCanvas for testing
    int maxHealth; //Manually set until OnEntityInit is created;
    void Awake(){
        attachedEntity.OnHealReceived += OnHealRecieved;
        attachedEntity.OnDamageTaken += OnDamageTaken;
        attachedEntity.OnEntityInit += OnEntityInit;
        fillAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        lockRotation();
        updateInterpolationLayer();
        updateTopLayer();
        shaking();
    }

    private void OnEntityInit(int test){
        maxHealth = attachedEntity.Health;
    }
    private void lockRotation(){
        Camera camera = Camera.main;
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.back, camera.transform.rotation * Vector3.up);
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
            time += Time.deltaTime;
            float shakeAmount = 0.1f * (float)damage / 4;
            float shakeSpeed = 50f * (float)damage / 4;
            if(shakeSpeed < 30f){
                shakeSpeed = 30f;
            }
            if(shakeAmount < 0.07f){
                shakeAmount = 0.07f;
            }
            gameObject.transform.localPosition = new Vector3(Mathf.Sin(Time.time * shakeSpeed) * shakeAmount,0,Mathf.Sin(Time.time * shakeSpeed) * shakeAmount);
            if(time >= 0.5){
                time = 0;
                shakeEffect = false;
                gameObject.transform.localPosition = new Vector3(0, 0, 0);
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
