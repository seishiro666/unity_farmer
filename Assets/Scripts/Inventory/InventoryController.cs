using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] List<FlowerData> items;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] GameObject slots;

    public void AddItem()
    {
        ClearSlots();

        foreach (var item in inventory.inventorySystem)
        {
            GameObject tempSlot = Instantiate(slotPrefab, slots.transform);
            tempSlot.GetComponent<InventorySlot>().SetupSlot(item.item.icon, item.count);   
        }
    }

    void ClearSlots()
    {
        int numOfChild = slots.transform.childCount;

        for (int i = 0; i < numOfChild; i++) 
        {
            Destroy(slots.transform.GetChild(i).gameObject);
        }
    }
}
