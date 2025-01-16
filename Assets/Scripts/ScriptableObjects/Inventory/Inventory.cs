using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventory", menuName = "ScriptableObject/InventorySystem")]
public class Inventory : ScriptableObject
{
    public List<InventorySystem> inventorySystem;        
}

[System.Serializable]
public class InventorySystem
{
    public FlowerData item;
    public bool isSeed;
    public int count;
}
