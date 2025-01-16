using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] List<GameObject> slotObjects;
    [SerializeField] GameObject itemPrefab;

    private void Start()
    {
        UpdateSlots();
    }

    public void UpdateSlots()
    {
        ClearSlots();

        for (int i = 0; i < inventory.inventorySystem.Count; i++)
        {
            if (i < slotObjects.Count)
            {
                InventorySystem item = inventory.inventorySystem[i];
                GameObject slot = slotObjects[i];
                GameObject tempItem = Instantiate(itemPrefab, slot.transform);
                InventoryItem inventoryItem = tempItem.GetComponent<InventoryItem>();
                inventoryItem.SetupSlot(item.item.icon, item.count);
            }
        }
    }

    private void ClearSlots()
    {
        foreach (GameObject slot in slotObjects)
        {
            if (slot.transform.childCount > 0)
            {
                Destroy(slot.transform.GetChild(0).gameObject);
            }
        }
    }
}
