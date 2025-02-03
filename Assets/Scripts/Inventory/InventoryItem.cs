using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

// Класс, отвечающий за предметы в инвентаре
public class InventoryItem : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] Image itemIcon; // Иконка предмета
    [SerializeField] TMP_Text countText; // Текст для отображения количества

    InventorySystem slotData; // Данные о предмете
    public FlowerData flowerData; // Данные о цветке
    public int itemCount = 0; // Количество предметов

    [HideInInspector] public Transform parentAfterDrag; // Родительский объект после перетаскивания

    // Метод для настройки слота
    public void SetupSlot(Sprite icon, int count, InventorySystem data)
    {
        slotData = data; // Устанавливаем данные о предмете
        itemIcon.sprite = icon; // Устанавливаем иконку
        itemCount = count; // Устанавливаем количество
        flowerData = data.item; // Устанавливаем данные о цветке
        countText.text = itemCount.ToString(); // Обновляем текст
    }

    // Метод для очистки слота
    public void ClearSlot()
    {
        Destroy(gameObject); // Удаляем объект
    }

    // Метод для обновления предмета
    public void RefreshItem(int count)
    {
        itemCount = count; // Устанавливаем новое количество
        countText.text = itemCount.ToString(); // Обновляем текст
        slotData.count = count; // Обновляем данные о предмете
    }

    // Метод для увеличения количества предметов
    public void IncreaseCount(int count)
    {
        itemCount += count; // Увеличиваем количество
        countText.text = itemCount.ToString(); // Обновляем текст
    }

    // Метод для получения данных о предмете
    public InventorySystem GetInventorySystemData()
    {
        return slotData; // Возвращаем данные о предмете
    }

    // Метод для начала перетаскивания
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent; // Сохраняем родительский объект
        transform.SetParent(transform.root); // Устанавливаем объект в корень
        transform.SetAsLastSibling(); // Устанавливаем объект последним
        itemIcon.raycastTarget = false; // Отключаем взаимодействие с иконкой
    }

    // Метод для перетаскивания
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; // Устанавливаем позицию объекта
    }

    // Метод для завершения перетаскивания
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag); // Возвращаем объект обратно
        itemIcon.raycastTarget = true; // Включаем взаимодействие с иконкой
    }
}