using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Класс, отвечающий за управление инвентарем игрока
public class InventoryController : MonoBehaviour
{
    [Header("Скрипты")]
    [SerializeField] BedController bedController; // Контроллер кровати

    [Header("Инвентарь")]
    [SerializeField] GameObject inventoryUI; // UI инвентаря
    public Inventory inventory; // Система инвентаря
    [SerializeField] List<GameObject> slotObjects; // Слоты для предметов
    [SerializeField] GameObject itemPrefab; // Префаб предмета
    int MaxStackSize = 32; // Максимальный размер стека
    Transform bedInventoryUI; // UI для инвентаря кровати

    [Header("Игрок")]
    public PlayerController playerController; // Контроллер игрока
    [SerializeField] UserData userData; // Данные пользователя

    [Header("Магазин")]
    [SerializeField] GameObject shopUI; // UI магазина
    [SerializeField] GameObject shopSlotPrefab; // Префаб слота магазина
    [SerializeField] GameObject shopSlotsUI; // UI слотов магазина
    public List<FlowerData> shopData; // Данные о цветах в магазине
    [SerializeField] GameObject shopItemPrefab; // Префаб предмета магазина
    [SerializeField] List<GameObject> shopObjects = new List<GameObject>(); // Список объектов магазина

    [Header("Продажа/Удаление")]
    [SerializeField] GameObject sellSlot; // Слот для продажи
    [SerializeField] GameObject deleteSlot; // Слот для удаления
    [SerializeField] GameObject bedSlot; // Слот для кровати

    public static Action<InventoryItem, GameObject, InventoryController> onBedBtnClick; // Событие нажатия на кнопку кровати

    private void Awake()
    {
        bedInventoryUI = inventoryUI.transform.GetChild(3); // Получаем UI инвентаря кровати
    }

    void Update()
    {
        HandleSellSlot(); // Обрабатываем слот для продажи
        HandleDeleteSlot(); // Обрабатываем слот для удаления
    }

    // Метод для обработки слота продажи
    void HandleSellSlot()
    {
        if (sellSlot.transform.childCount > 0)
        {
            InventoryItem itemToSell = sellSlot.transform.GetChild(0).GetComponent<InventoryItem>(); // Получаем предмет для продажи
            InventorySystem itemData = itemToSell.GetInventorySystemData(); // Получаем данные о предмете
            Destroy(sellSlot.transform.GetChild(0).gameObject); // Удаляем предмет из слота
            playerController.AddMoney(Mathf.CeilToInt(itemData.item.price * (itemData.count / 4))); // Добавляем деньги игроку
        }
    }

    // Метод для обработки слота удаления
    void HandleDeleteSlot()
    {
        if (deleteSlot.transform.childCount > 0)
        {
            InventoryItem itemToDelete = deleteSlot.transform.GetChild(0).GetComponent<InventoryItem>(); // Получаем предмет для удаления
            InventorySystem itemData = itemToDelete.GetInventorySystemData(); // Получаем данные о предмете
            Destroy(deleteSlot.transform.GetChild(0).gameObject); // Удаляем предмет из слота
            inventory.inventorySystem.Remove(itemData); // Удаляем предмет из инвентаря
        }
    }

    // Метод для обновления слотов инвентаря
    void UpdateSlots(int typeOfItems = 1, string actionBtnText = "1")
    {
        ClearSlots(); // Очищаем слоты
        InventorySystem itemData = null; // Данные о предмете

        // Обновляем слоты в инвентаре
        for (int i = 0; i < inventory.inventorySystem.Count; i++)
        {
            if (i < slotObjects.Count)
            {
                itemData = inventory.inventorySystem[i];

                if (itemData.count <= 0)
                {
                    inventory.inventorySystem.RemoveAt(i); // Удаляем пустые предметы
                    break;
                }

                GameObject slot = slotObjects[i];

                // Проверяем тип предмета и создаем соответствующий UI
                if (typeOfItems == 0 && itemData.isSeed)
                {
                    GameObject tempItem = Instantiate(itemPrefab, slot.transform);
                    InventoryItem inventoryItem = tempItem.GetComponent<InventoryItem>();
                    inventoryItem.SetupSlot(itemData.item.seedIcon, itemData.count, itemData);
                }
                else if (typeOfItems != 0)
                {
                    GameObject tempItem = Instantiate(itemPrefab, slot.transform);
                    InventoryItem inventoryItem = tempItem.GetComponent<InventoryItem>();
                    Sprite itemIcon = itemData.isSeed ? itemData.item.seedIcon : itemData.item.icon;
                    inventoryItem.SetupSlot(itemIcon, itemData.count, itemData);
                }
            }

            inventoryUI.SetActive(true); // Активируем UI инвентаря
        }

        // Обновляем текст кнопки действия
        bedInventoryUI.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = actionBtnText;

        // Проверяем тип предметов для отображения UI
        if (typeOfItems == 0)
        {
            inventoryUI.transform.localPosition = new Vector3(200, 0, 0);
            bedInventoryUI.gameObject.SetActive(true);
            bedInventoryUI.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            bedInventoryUI.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SubToBedBtnEvent(itemData); });
        }
        else
        {
            inventoryUI.transform.localPosition = Vector3.zero;
            bedInventoryUI.gameObject.SetActive(false);
        }
    }

    // Метод для обновления инвентаря по нажатию кнопки
    public void UpdateInventoryFromBtn()
    {
        UpdateSlots(); // Обновляем слоты
    }

    // Метод для добавления предмета в инвентарь
    public void AddItemToInventory(InventorySystem newItem)
    {
        bool itemAdded = false; // Флаг для проверки добавления предмета

        // Проверяем, есть ли уже такой предмет в инвентаре
        foreach (var existingItem in inventory.inventorySystem)
        {
            if (existingItem.item == newItem.item && existingItem.isSeed == newItem.isSeed)
            {
                int availableSpace = MaxStackSize - existingItem.count; // Проверяем доступное место

                if (availableSpace > 0)
                {
                    int amountToAdd = Mathf.Min(newItem.count, availableSpace); // Вычисляем количество для добавления
                    existingItem.count += amountToAdd; // Увеличиваем количество существующего предмета
                    newItem.count -= amountToAdd; // Уменьшаем количество нового предмета

                    if (newItem.count <= 0)
                    {
                        itemAdded = true; // Предмет добавлен
                        break;
                    }
                }
            }
        }

        // Добавляем новый предмет в инвентарь
        while (newItem.count > 0 && !itemAdded)
        {
            InventorySystem newStack = new InventorySystem
            {
                item = newItem.item,
                count = Mathf.Min(newItem.count, MaxStackSize),
                isSeed = newItem.isSeed
            };

            newItem.count -= newStack.count; // Уменьшаем количество нового предмета
            inventory.inventorySystem.Add(newStack); // Добавляем новый стек в инвентарь
        }
    }

    // Метод для обработки нажатия кнопки кровати
    void SubToBedBtnEvent(InventorySystem itemInventoryData)
    {
        if (bedSlot.transform.childCount > 0 && bedSlot.transform.GetChild(0).GetComponent<InventoryItem>().itemCount >= 4)
        {
            onBedBtnClick?.Invoke(bedSlot.transform.GetChild(0).GetComponent<InventoryItem>(), bedSlot, gameObject.GetComponent<InventoryController>()); // Вызываем событие нажатия на кнопку кровати
            UpdateSlots(); // Обновляем слоты
            if (bedSlot.transform.childCount > 0)
            {
                Destroy(bedSlot.transform.GetChild(0).gameObject); // Удаляем предмет из слота кровати
            }
        }
    }

    // Метод для очистки слотов
    void ClearSlots()
    {
        foreach (GameObject slotObject in slotObjects)
        {
            if (slotObject.transform.childCount > 0)
            {
                for (int i = 0; i < slotObject.transform.childCount; i++)
                {
                    Destroy(slotObject.transform.GetChild(i).gameObject); // Удаляем предметы из слотов
                }
            }
        }

        // Очищаем слоты для продажи, удаления и кровати
        if (sellSlot.transform.childCount > 0)
        {
            Destroy(sellSlot.transform.GetChild(0).gameObject);
        }

        if (deleteSlot.transform.childCount > 0)
        {
            Destroy(deleteSlot.transform.GetChild(0).gameObject);
        }

        if (bedSlot.transform.childCount > 0)
        {
            Destroy(bedSlot.transform.GetChild(0).gameObject);
        }
    }

    // Метод для сбора предметов из магазина
    public void CollectItemsFromShop()
    {
        if (shopObjects.Count != 0)
        {
            foreach (GameObject slotObject in shopObjects)
            {
                Destroy(slotObject); // Удаляем старые слоты
            }
        }

        // Создаем новые слоты для предметов в магазине
        for (int j = 0; j < shopData.Count; j++)
        {
            GameObject tempSlot = Instantiate(shopSlotPrefab, shopSlotsUI.transform);
            shopObjects.Add(tempSlot);
        }

        // Заполняем слоты предметами
        for (int i = 0; i < shopObjects.Count; i++)
        {
            GameObject tempItem = Instantiate(shopItemPrefab, shopObjects[i].transform);
            InventoryItem tempInvItem = tempItem.GetComponent<InventoryItem>();
            tempInvItem.flowerData = shopData[i];

            InventorySystem tempInvSystem = new InventorySystem
            {
                item = shopData[i],
                isSeed = true,
                count = 50
            };

            tempInvItem.SetupSlot(tempInvSystem.item.seedIcon, tempInvSystem.count, tempInvSystem); // Настраиваем слот
            tempInvItem.RefreshItem(tempInvSystem.item.price); // Обновляем цену
            shopObjects[i].GetComponent<Button>().onClick.AddListener(delegate { BuyItemFromShop(tempInvSystem); }); // Добавляем обработчик нажатия
        }

        shopUI.SetActive(true); // Активируем UI магазина
    }

    // Метод для покупки предмета из магазина
    void BuyItemFromShop(InventorySystem tempInvSystem)
    {
        if (playerController.money >= tempInvSystem.item.price) // Проверяем, достаточно ли денег
        {
            tempInvSystem.count = 4; // Устанавливаем количество
            playerController.AddMoney(-tempInvSystem.item.price); // Уменьшаем деньги игрока
            AddItemToInventory(tempInvSystem); // Добавляем предмет в инвентарь
        }
    }

    // Метод для обработки нажатия на кровать
    void OnBedClicked(int numOfItem, string actionBtnText)
    {
        UpdateSlots(numOfItem, actionBtnText); // Обновляем слоты
    }

    private void OnEnable()
    {
        BedWork.onBedClick += OnBedClicked; // Подписываемся на событие нажатия на кровать
    }

    private void OnDisable()
    {
        BedWork.onBedClick -= OnBedClicked; // Отписываемся от события нажатия на кровать
    }
}