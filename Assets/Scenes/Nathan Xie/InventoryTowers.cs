using System.Collections.Generic;
using UnityEngine;

public class InventoryTowers : MonoBehaviour
{
    [SerializeField] private  TowerData[] towerIcons;
    [SerializeField] private DraggableIcons icon;
    private List<DraggableIcons> icons = new List<DraggableIcons>();
    void Awake()
    {
        setIcons();
    }
    public void setIcons(){
        while (icons.Count != 0) {
            icons[0].DestroySelf();
            icons.RemoveAt(0);
        }
        int gridCount = 0;
        foreach (TowerData image in towerIcons) {
            DraggableIcons towerSlot = Instantiate(icon);
            towerSlot.towerData = image;
            towerSlot.transform.SetParent(transform.GetChild(gridCount));
            towerSlot.setImage();
            icons.Add(towerSlot);
            gridCount++;
        }
    }
}
