using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ManaUIManager : MonoBehaviour {
    [SerializeField] private Slider slider;
    [SerializeField] private bool debugMode = false;

    public void Update() {
        if (debugMode && Input.GetMouseButtonDown(0)) {
            UpdateBar(Random.Range(0f, 100f), 100f);
        }
    }

    public void UpdateBar(float currMana, float maxMana) {
        float valueRatio = currMana / maxMana;
        DOTween.To(() => slider.value, x => slider.value = x, valueRatio, 0.25f);
    }
}
