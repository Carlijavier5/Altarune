using UnityEngine;
using UnityEngine.UI;

public class PHSelector : MonoBehaviour {

    [SerializeField] private Sprite unselected, selected;
    [SerializeField] private Image battery;
    [SerializeField] private Image[] towers;

    public void SetSelectedImage(SummonController.SelectionType selectionType, int index) {
        switch (selectionType) {
            case SummonController.SelectionType.None:
                battery.sprite = unselected;
                foreach (Image tower in towers) tower.sprite = unselected;
                break;
            case SummonController.SelectionType.Battery:
                battery.sprite = selected;
                foreach (Image tower in towers) tower.sprite = unselected;
                break;
            case SummonController.SelectionType.Tower:
                battery.sprite = unselected;
                foreach (Image tower in towers) tower.sprite = unselected;
                towers[index].sprite = selected;
                break;
        }
    }
}
