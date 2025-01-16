using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryController : MonoBehaviour
{
    [SerializeField] GameObject inventoryUI;
    [SerializeField] Inventory inventory;
    [SerializeField] List<GameObject> slotObjects;
    [SerializeField] GameObject bedSlot;
    [SerializeField] GameObject itemPrefab;
    
    int MaxStackSize = 32;
    Transform bedInventoryUI;

    public static Action<InventoryItem> onBedBtnClick;

    private void Awake()
    {
        bedInventoryUI = inventoryUI.transform.GetChild(3);
    }

    void UpdateSlots(bool onlySeed, string actionBtnText)
    {
        ClearSlots();

        for (int i = 0; i < inventory.inventorySystem.Count; i++)
        {
            if (i < slotObjects.Count)
            {
                InventorySystem itemData = inventory.inventorySystem[i];
                GameObject slot = slotObjects[i];

                if (onlySeed && itemData.isSeed)
                {
                    GameObject tempItem = Instantiate(itemPrefab, slot.transform);
                    InventoryItem inventoryItem = tempItem.GetComponent<InventoryItem>();
                    inventoryItem.SetupSlot(itemData.item.seedIcon, itemData.count, itemData.item);
                }
                else if (!onlySeed)
                {
                    GameObject tempItem = Instantiate(itemPrefab, slot.transform);
                    InventoryItem inventoryItem = tempItem.GetComponent<InventoryItem>();
                    Sprite itemIcon = itemData.isSeed ? itemData.item.seedIcon : itemData.item.icon;
                    inventoryItem.SetupSlot(itemIcon, itemData.count, itemData.item);
                }
            }
        }

        if (onlySeed)
        {
            inventoryUI.transform.localPosition = new Vector3(200, 0, 0);
            bedInventoryUI.gameObject.SetActive(true);
            bedInventoryUI.GetChild(1).GetComponent<Button>().onClick.AddListener( delegate { SubToBedBtnEvent(); } );
            bedInventoryUI.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = actionBtnText;
        }
        else
        {
            inventoryUI.transform.localPosition = Vector3.zero;
            bedInventoryUI.gameObject.SetActive(false);
        }

        inventoryUI.SetActive(true);
    }

    private void AddItemToSlots(FlowerData item, int count)
    {
        while (count > 0)
        {
            bool addToExistSlot = false;
            foreach (var slotObject in slotObjects)
            {
                if (slotObject.transform.childCount > 0)
                {
                    InventoryItem inventoryItem = slotObject.transform.GetChild(0).GetComponent<InventoryItem>();
                    if (inventoryItem != null && 
                       (inventoryItem.GetComponent<Image>().sprite == item.icon || inventoryItem.GetComponent<Image>().sprite == item.seedIcon) && 
                        inventoryItem.itemCount < MaxStackSize)
                    {
                        int spaceLeft = MaxStackSize - inventoryItem.itemCount;
                        int amountToAdd = Mathf.Min(count, spaceLeft);
                        inventoryItem.IncreaseCount(amountToAdd);
                        count -= amountToAdd;
                        addToExistSlot = true;
                        break;
                    }
                }
            }

            if (!addToExistSlot)
            {
                foreach (var slotObject in slotObjects)
                {
                    if (slotObject.transform.childCount == 0)
                    {
                        GameObject newItem = Instantiate(itemPrefab, slotObject.transform);
                        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
                        inventoryItem.SetupSlot(item.icon, Mathf.Min(count, MaxStackSize), item);
                        count -= Mathf.Min(count, MaxStackSize);
                        break;
                    }
                }
            }

            if (!addToExistSlot && count > 0)
            {
                Debug.Log("Полный инвентарь");
                break;
            }
        }

        UpdateSlots(false, "1");
    }

    void SubToBedBtnEvent()
    {
        if (bedSlot.transform.childCount > 0 && bedSlot.transform.GetChild(0).GetComponent<InventoryItem>().itemCount >= 4) 
            onBedBtnClick?.Invoke(bedSlot.transform.GetChild(0).GetComponent<InventoryItem>());
    }

    void ClearSlots()
    {
        foreach (GameObject slotObject in slotObjects)
        {
            if (slotObject.transform.childCount > 0)
            {
                Destroy(slotObject.transform.GetChild(0).gameObject);
            }
        }

        if (bedSlot.transform.childCount > 0)
        {
            Destroy(bedSlot.transform.GetChild(0).gameObject);
        }
    }

    void OnBedClicked(bool onlySeed, string actionBtnText)
    {
        UpdateSlots(onlySeed, actionBtnText);
    }

    private void OnEnable()
    {
        BedWork.onBedClick += OnBedClicked;
    }

    private void OnDisable()
    {
        BedWork.onBedClick -= OnBedClicked;
    }
}
