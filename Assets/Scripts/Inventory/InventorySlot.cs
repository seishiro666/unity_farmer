using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Класс, отвечающий за слоты инвентаря
public class InventorySlot : MonoBehaviour, IDropHandler
{
    // Метод для обработки события перетаскивания
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0) // Проверяем, пустой ли слот
        {
            GameObject dropped = eventData.pointerDrag; // Получаем перетаскиваемый объект
            InventoryItem item = dropped.GetComponent<InventoryItem>(); // Получаем компонент InventoryItem
            item.parentAfterDrag = transform; // Устанавливаем новый родительский объект
        }
    }
}