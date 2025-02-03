using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Перечисление состояний кровати
public enum State
{
    Empty,
    SeedPlanted,
    Sprout,
    MaturePlant,
    Harvest,
    End
}

// Класс, отвечающий за работу с кроватью
public class BedWork : MonoBehaviour
{
    public static Action<int, string> onBedClick; // Событие нажатия на кровать

    [SerializeField] GameObject flowerPos; // Позиция для цветов

    [SerializeField] State currentState = State.Empty; // Текущее состояние кровати
    FlowerData flowerData; // Данные о цветке
    InventoryItem flowerItemData; // Данные о предмете цветка
    GameObject flowerObj; // Объект цветка
    List<GameObject> flowersList = new List<GameObject>(); // Список цветов
    [SerializeField] bool haveSeed = false; // Наличие семян
    GameObject bedSlot; // Слот для кровати
    GameObject itemPrefab; // Префаб предмета
    InventoryController inventoryController; // Контроллер инвентаря
    InventorySystem inventorySystem; // Система инвентаря

    // Метод для настройки кровати
    public void SetupBed()
    {
        if (currentState == State.Empty)
        {
            onBedClick?.Invoke(0, "��������"); // Вызываем событие нажатия на кровать
            OnSetup(); // Выполняем настройку
        }
        else if (currentState == State.MaturePlant)
        {
            CollectSeeds(); // Собираем семена
            inventorySystem = null; // Очищаем систему инвентаря
            SwapState(); // Меняем состояние
        }
        else if (currentState == State.End)
        {
            CollectFlowers(); // Собираем цветы
            ClearBed(); // Очищаем кровать
        }
    }

    // Метод для начала роста
    void StartGrowth(InventoryItem itemData, GameObject bedSlotUI, InventoryController invController)
    {
        inventoryController = invController; // Устанавливаем контроллер инвентаря
        flowerItemData = itemData; // Устанавливаем данные о предмете
        flowerData = flowerItemData.flowerData; // Получаем данные о цветке
        bedSlot = bedSlotUI; // Устанавливаем слот для кровати

        // Проверяем, достаточно ли предметов для роста
        if (itemData.itemCount >= 4)
        {
            itemData.itemCount -= 4; // Уменьшаем количество предметов

            // Проверяем, нужно ли удалять предмет из инвентаря
            if (itemData.itemCount <= 0)
            {
                inventoryController.inventory.inventorySystem.Remove(itemData.GetInventorySystemData());
                itemData.ClearSlot(); // Очищаем слот
            }
            else
            {
                itemData.RefreshItem(itemData.itemCount); // Обновляем предмет
            }

            flowerObj = flowerData.model; // Получаем модель цветка

            // Создаем цветы
            for (int i = 0; i < 4; i++)
            {
                GameObject tempFlower = Instantiate(flowerObj, flowerPos.transform.GetChild(i));
                flowersList.Add(tempFlower); // Добавляем цветок в список
            }
            currentState = State.SeedPlanted; // Устанавливаем состояние

            OnEndSetup(); // Завершаем настройку

            StartCoroutine(StartGrowthProcess(itemData)); // Запускаем процесс роста
        }
    }

    // Корутин для процесса роста
    IEnumerator StartGrowthProcess(InventoryItem itemData)
    {
        yield return new WaitForSeconds(5f); // Ждем 5 секунд

        // Меняем состояние цветка на "Саженец"
        foreach (GameObject flower in flowersList)
        {
            flower.transform.GetChild(0).gameObject.SetActive(false);
            flower.transform.GetChild(1).gameObject.SetActive(true);
        }
        currentState = State.Sprout;

        yield return new WaitForSeconds(flowerData.seedGrowth); // Ждем время роста семян

        // Меняем состояние цветка на "Взрослый"
        foreach (GameObject flower in flowersList)
        {
            flower.transform.GetChild(1).gameObject.SetActive(false);
            flower.transform.GetChild(2).gameObject.SetActive(true);
        }
        currentState = State.MaturePlant;

        haveSeed = true; // Устанавливаем наличие семян
        RndSeedSpawn(); // Запускаем спавн семян
    }

    // Метод для сбора семян
    void CollectSeeds()
    {
        inventoryController.AddItemToInventory(inventorySystem); // Добавляем семена в инвентарь
    }

    // Метод для случайного спавна семян
    void RndSeedSpawn()
    {
        List<FlowerData> neighboringSeeds = new List<FlowerData>(); // Список соседних семян
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right }; // Направления

        // Проверяем соседние кровати
        foreach (var direction in directions)
        {
            BedWork neighboringBed = GetNeighboringBed(direction); // Получаем соседнюю кровать

            if (neighboringBed != null && neighboringBed.currentState == State.SeedPlanted)
            {
                neighboringSeeds.Add(neighboringBed.GetFlowerData()); // Добавляем данные о цветке
            }
        }

        // Если есть соседние семена, комбинируем или выбираем случайное семя
        if (neighboringSeeds.Count > 0)
        {
            bool combineSeeds = UnityEngine.Random.Range(0f, 1f) < 0.5f;

            if (combineSeeds)
            {
                CombineSeeds(neighboringSeeds); // Комбинируем семена
            }
            else
            {
                int randomSeedIndex = UnityEngine.Random.Range(0, neighboringSeeds.Count);
                InventorySystem seedInventorySystem = new InventorySystem
                {
                    item = neighboringSeeds[randomSeedIndex],
                    count = 4,
                    isSeed = true
                };

                inventorySystem = seedInventorySystem; // Устанавливаем систему инвентаря
            }
        }
        else
        {
            int seedNum = 0;
            InventorySystem seedInventorySystem = new InventorySystem
            {
                item = flowerData.potentialSeeds[seedNum],
                count = 4,
                isSeed = true
            };

            inventorySystem = seedInventorySystem; // Устанавливаем систему инвентаря
        }
    }

    // Метод для получения соседней кровати
    BedWork GetNeighboringBed(Vector2 direction)
    {
        Vector3 neighborPosition = transform.position + new Vector3(direction.x, 0, direction.y); // Вычисляем позицию соседа

        BedWork[] allBeds = FindObjectsOfType<BedWork>(); // Получаем все кровати

        // Проверяем расстояние до соседних кроватей
        foreach (var bed in allBeds)
        {
            if (Vector3.Distance(neighborPosition, bed.transform.position) < 5f)
            {
                return bed; // Возвращаем соседнюю кровать
            }
        }

        return null; // Если соседей нет, возвращаем null
    }

    // Метод для комбинирования семян
    void CombineSeeds(List<FlowerData> neighboringSeeds)
    {
        int randomNeighborIndex = UnityEngine.Random.Range(0, neighboringSeeds.Count);
        FlowerData selectedSeed = neighboringSeeds[randomNeighborIndex];

        string currentFlowerName = flowerData.name.Replace("Flower", "");
        string neighborFlowerName = selectedSeed.name.Replace("Flower", "");

        FlowerData combinedSeed = flowerData.potentialSeeds.Find(seed =>
            seed.name.Contains(currentFlowerName) && seed.name.Contains(neighborFlowerName));

        if (combinedSeed != null)
        {
            InventorySystem combinedSeedSystem = new InventorySystem
            {
                item = combinedSeed,
                count = 4,
                isSeed = true
            };

            inventorySystem = combinedSeedSystem; // Устанавливаем систему инвентаря
        }
    }

    // Метод для смены состояния
    void SwapState()
    {
        haveSeed = false; // Очищаем наличие семян
        StartCoroutine(EndGrowth()); // Запускаем корутин для завершения роста
    }

    // Корутин для завершения роста
    IEnumerator EndGrowth()
    {
        currentState = State.Harvest; // Устанавливаем состояние "Сбор урожая"

        if (!haveSeed)
        {
            yield return new WaitForSeconds(flowerData.growth); // Ждем время роста

            // Меняем состояние цветка на "Собранный"
            foreach (GameObject flower in flowersList)
            {
                flower.transform.GetChild(2).gameObject.SetActive(false);
                flower.transform.GetChild(3).gameObject.SetActive(true);
            }
            currentState = State.End; // Устанавливаем состояние "Конец"

            InventorySystem seedInventorySystem = new InventorySystem();
            seedInventorySystem.item = flowerData; // Устанавливаем данные о цветке
            seedInventorySystem.count = 4; // Количество семян
            seedInventorySystem.isSeed = false; // Устанавливаем, что это не семена
            inventorySystem = seedInventorySystem; // Устанавливаем систему инвентаря
        }
    }

    // Метод для сбора цветов
    void CollectFlowers()
    {
        inventoryController.playerController.AddExperience(inventorySystem.item.expReward); // Добавляем опыт игроку
        inventoryController.AddItemToInventory(inventorySystem); // Добавляем цветы в инвентарь
    }

    // Метод для очистки кровати
    void ClearBed()
    {
        StopAllCoroutines(); // Останавливаем все корутины

        currentState = State.Empty; // Устанавливаем состояние "Пусто"
        haveSeed = false; // Очищаем наличие семян
        inventorySystem = null; // Очищаем систему инвентаря

        // Удаляем все цветы с кровати
        foreach (Transform flower in flowerPos.transform)
        {
            foreach (Transform child in flower)
            {
                Destroy(child.gameObject);
            }
        }
        flowersList.Clear(); // Очищаем список цветов

        // Удаляем слот для кровати
        if (bedSlot != null && bedSlot.transform.childCount > 0)
        {
            Destroy(bedSlot.transform.GetChild(0).gameObject);
        }

        flowerData = null; // Очищаем данные о цветке
        flowerItemData = null; // Очищаем данные о предмете
        flowerObj = null; // Очищаем объект цветка

        OnEndSetup(); // Завершаем настройку
    }

    // Метод для получения данных о цветке
    public FlowerData GetFlowerData()
    {
        return flowerData; // Возвращаем данные о цветке
    }

    private void OnSetup()
    {
        InventoryController.onBedBtnClick += StartGrowth; // Подписываемся на событие нажатия на кровать
    }

    private void OnEndSetup()
    {
        InventoryController.onBedBtnClick -= StartGrowth; // Отписываемся от события нажатия на кровать
    }
}