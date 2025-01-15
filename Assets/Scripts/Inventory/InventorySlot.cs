using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDragHandler, IDropHandler
{
    Image itemIcon;
    int itemCount = 0;
    TMP_Text countText;

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

    public void IncreaseCount(int count)
    {
        itemCount += count;
        countText.text = count.ToString();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }   
    public void OnDrop(PointerEventData eventData)
    {
        eventData.pointerDrag.transform.position = transform.position;
    }
}
