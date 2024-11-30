using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryTowerSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData) {
        if(transform.childCount == 0) {
            GameObject dropped = eventData.pointerDrag;
            DraggableIcons draggableIcon = dropped.GetComponent<DraggableIcons>();
            draggableIcon.parentAfterDrag = transform;
        }
    }
}
