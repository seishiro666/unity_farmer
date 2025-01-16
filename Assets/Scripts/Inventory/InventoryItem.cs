using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class InventoryItem : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] Image itemIcon;
    [SerializeField] TMP_Text countText;

    public FlowerData flowerData;
    public int itemCount = 0;

    [HideInInspector] public Transform parentAfterDrag;

    public void SetupSlot(Sprite icon, int count, FlowerData data)
    {
        itemIcon.sprite = icon;
        itemCount = count;
        flowerData = data;
        countText.text = itemCount.ToString();
    }

    public void ClearSlot()
    {
        itemIcon.sprite = null;
        countText.text = "";
        itemCount = -1;
    }

    public void IncreaseCount(int count)
    {
        itemCount += count;
        countText.text = itemCount.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        itemIcon.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        itemIcon.raycastTarget = true;
    }
}
