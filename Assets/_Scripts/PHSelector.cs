using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PHSelector : MonoBehaviour {

    [SerializeField] private Sprite unselected, selected;
    [SerializeField] private Image battery, tower;

    public void SetSelectedImage(SummonController.SelectionType selectionType) {
        switch (selectionType) {
            case SummonController.SelectionType.None:
                battery.sprite = unselected;
                tower.sprite = unselected;
                break;
            case SummonController.SelectionType.Battery:
                battery.sprite = selected;
                tower.sprite = unselected;
                break;
            case SummonController.SelectionType.Tower:
                battery.sprite = unselected;
                tower.sprite = selected;
                break;
        }
    }
}
