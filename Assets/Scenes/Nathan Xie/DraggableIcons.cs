using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableIcons : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] public TowerData towerData;
    public Image image;
    [HideInInspector] public Transform parentAfterDrag;
    public void setImage(){
        image.sprite = towerData.icon;
    }
    public void OnBeginDrag(PointerEventData eventData){
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }
    public void OnDrag(PointerEventData eventData){
        transform.position = Input.mousePosition;
    }
    public void OnEndDrag(PointerEventData eventData){
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }
    public void DestroySelf(){
        Destroy(gameObject);
        Destroy(this);
    }
}
