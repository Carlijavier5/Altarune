using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerSelectionMenu : MonoBehaviour
{
    [SerializeField] private Canvas equippedTowers;
    [SerializeField] private InventoryTowers inventoryTowers;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private Button resetButton;
    // Start is called before the first frame update
    void Awake(){
        group.alpha = 0;
        group.interactable = false;
        resetButton.onClick.AddListener(() => Reset());
    }
    public HashSet<TowerData> getTowerSelection() {
        HashSet<TowerData> towerHash = new HashSet<TowerData>();
        for(int i = 0;i < equippedTowers.transform.childCount;i++) {
            if(equippedTowers.transform.GetChild(i).childCount == 1){
                towerHash.Add(equippedTowers.transform.GetChild(i).GetChild(0).GetComponent<DraggableIcons>().towerData);
            }
        }
        return towerHash;
    }
    public void Reset(){
        inventoryTowers.setIcons();
    }
}
