using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PHSelector : MonoBehaviour {

    [SerializeField] private Sprite unselected, selected;
    [SerializeField] private Image battery;
    [SerializeField] private Image[] towers;

    public void SetSelectedImage(SummonType selectionType, int index) {
        if (index >= towers.Length) return;
        switch (selectionType) {
            case SummonType.None:
                battery.sprite = unselected;
                foreach (Image tower in towers) {
                    tower.sprite = unselected;
                }
                break;
            case SummonType.Battery:
                battery.sprite = selected;
                foreach (Image tower in towers) {
                    tower.sprite = unselected;
                }
                break;
            case SummonType.Tower:
                battery.sprite = unselected;
                foreach (Image tower in towers) {
                    tower.sprite = unselected;
                }
                towers[index].sprite = selected;
                break;
        }
    }
}
