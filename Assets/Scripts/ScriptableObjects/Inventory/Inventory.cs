using System.Collections.Generic;
using UnityEngine;

// Класс, отвечающий за инвентарь игрока
[CreateAssetMenu(fileName = "NewInventory", menuName = "ScriptableObject/InventorySystem")]
public class Inventory : ScriptableObject
{
    public List<InventorySystem> inventorySystem; // Список предметов в инвентаре
}

// Класс, представляющий предмет в инвентаре
[System.Serializable]
public class InventorySystem
{
    public FlowerData item; // Данные о цветке (предмете)
    public bool isSeed; // Является ли предмет семенем
    public int count; // Количество предметов
}