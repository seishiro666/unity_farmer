using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class InventoryItem : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    Image itemIcon;
    int itemCount = 0;
    TMP_Text countText;

    [HideInInspector] public Transform parentAfterDrag;

    private void Awake()
    {
        itemIcon = transform.GetChild(0).GetComponent<Image>();
        countText = transform.GetChild(1).GetComponent<TMP_Text>();
    }

    public void SetupSlot(Sprite icon, int count)
    {
        itemIcon.sprite = icon;
        itemCount = count;
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
