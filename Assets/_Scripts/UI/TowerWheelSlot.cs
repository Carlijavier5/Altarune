using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerWheelSlot : MonoBehaviour {

    [SerializeField] private Sprite selectedBg, unselectedBG;
    [SerializeField] private Image icon, background;

    public void Init(SummonData data) {
        icon.sprite = data.icon;
    }

    public void Toggle(bool on) => gameObject.SetActive(on);

    public void ToggleSelection(bool on) {
        Sprite sprite = on ? selectedBg : unselectedBG;
        background.sprite = sprite;
    }
}