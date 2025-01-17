using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;
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

    public static Action<InventoryItem, GameObject, GameObject, InventoryController, InventorySystem> onBedBtnClick;

    private void Awake()
    {
        bedInventoryUI = inventoryUI.transform.GetChild(3);
    }

    void UpdateSlots(int typeOfItems, string actionBtnText)
    {
        ClearSlots();
        InventorySystem itemData = null;

        for (int i = 0; i < inventory.inventorySystem.Count; i++)
        {
            if (i < slotObjects.Count)
            {
                itemData = inventory.inventorySystem[i];
                GameObject slot = slotObjects[i];

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
        }

        if (typeOfItems <= 1)
        {
            inventoryUI.transform.localPosition = new Vector3(200, 0, 0);
            bedInventoryUI.gameObject.SetActive(true);
            bedInventoryUI.GetChild(1).GetComponent<Button>().onClick.AddListener( delegate { SubToBedBtnEvent(itemData); } );
            bedInventoryUI.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = actionBtnText;
        }
        else
        {
            inventoryUI.transform.localPosition = Vector3.zero;
            bedInventoryUI.gameObject.SetActive(false);
        }

        inventoryUI.SetActive(true);
    }

    public void AddItemToBedSlot(InventorySystem item, int count, bool isBedAdd)
    {
        if (bedSlot.transform.childCount == 0)
        {
            GameObject newItem = Instantiate(itemPrefab, bedSlot.transform);
            InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
            inventoryItem.SetupSlot(item.item.icon, Mathf.Min(count, MaxStackSize), item);
            count -= Mathf.Min(count, MaxStackSize);
        }
    }

    void SubToBedBtnEvent(InventorySystem itemInventoryData)
    {
        if (bedSlot.transform.childCount > 0 && bedSlot.transform.GetChild(0).GetComponent<InventoryItem>().itemCount >= 4) 
            onBedBtnClick?.Invoke(bedSlot.transform.GetChild(0).GetComponent<InventoryItem>(), 
                bedSlot, itemPrefab, gameObject.GetComponent<InventoryController>(), itemInventoryData);
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

    void OnBedClicked(int numOfItem, string actionBtnText)
    {
        UpdateSlots(numOfItem, actionBtnText);
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
