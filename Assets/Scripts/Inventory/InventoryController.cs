using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;
using static UnityEditor.Progress;

public class InventoryController : MonoBehaviour
{
    [SerializeField] GameObject inventoryUI;
    public Inventory inventory;
    [SerializeField] List<GameObject> slotObjects;
    [SerializeField] GameObject bedSlot;
    [SerializeField] GameObject itemPrefab;
    
    int MaxStackSize = 32;
    Transform bedInventoryUI;

    public static Action<InventoryItem, GameObject, InventoryController> onBedBtnClick;

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

                if (itemData.count <= 0)
                {
                    inventory.inventorySystem.RemoveAt(i);
                    break;
                }

                GameObject slot = slotObjects[i];

                if (typeOfItems == 0 && itemData.isSeed)
                {
                    GameObject tempItem = Instantiate(itemPrefab, slot.transform);
                    InventoryItem inventoryItem = tempItem.GetComponent<InventoryItem>();
                    inventoryItem.SetupSlot(itemData.item.seedIcon, itemData.count, itemData);
                    Debug.Log(1);
                }
                else if (typeOfItems != 0)
                {
                    GameObject tempItem = Instantiate(itemPrefab, slot.transform);
                    InventoryItem inventoryItem = tempItem.GetComponent<InventoryItem>();
                    Sprite itemIcon = itemData.isSeed ? itemData.item.seedIcon : itemData.item.icon;
                    inventoryItem.SetupSlot(itemIcon, itemData.count, itemData);
                    Debug.Log(2);
                }
            }

            inventoryUI.SetActive(true);
        }

        if (typeOfItems <= 1)
        {
            inventoryUI.transform.localPosition = new Vector3(200, 0, 0);
            bedInventoryUI.gameObject.SetActive(true);
            bedInventoryUI.GetChild(1).GetComponent<Button>().onClick.AddListener( delegate { SubToBedBtnEvent(itemData); } );
            bedInventoryUI.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = actionBtnText;
            inventoryUI.SetActive(true);
        }
        else
        {
            inventoryUI.transform.localPosition = Vector3.zero;
            inventoryUI.gameObject.SetActive(false);
            bedInventoryUI.gameObject.SetActive(false);
        }

        ClearSlots();
    }

    public void AddItemToBedSlot(InventorySystem item, BedWork currentBed, bool isEnd)
    {
        if (bedSlot.transform.childCount == 1)
        {
            Destroy(bedSlot.transform.GetChild(0).gameObject);
        }

        GameObject newItem = Instantiate(itemPrefab, bedSlot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();

        if (!isEnd)
        {
            inventoryItem.SetupSlot(item.item.seedIcon, item.count, item);
        } else
        {
            inventoryItem.SetupSlot(item.item.icon, item.count, item);
        }

        bedInventoryUI.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
        bedInventoryUI.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { PickupItemFromBedSlot(item, currentBed, isEnd); });
    }

    void PickupItemFromBedSlot(InventorySystem invSystem, BedWork curBed, bool isEnd)
    {
        inventory.inventorySystem.Add(invSystem);
        Destroy(bedSlot.transform.GetChild(0).gameObject);
        UpdateSlots(2, "1");

        if (!isEnd)
        {
            curBed.SwapState();
        } else
        {
            ResetState();
            curBed.ClearBed();
        }
    }

    void SubToBedBtnEvent(InventorySystem itemInventoryData)
    {
        if (bedSlot.transform.childCount > 0 && bedSlot.transform.GetChild(0).GetComponent<InventoryItem>().itemCount >= 4)
        {
            onBedBtnClick?.Invoke(bedSlot.transform.GetChild(0).GetComponent<InventoryItem>(), bedSlot, gameObject.GetComponent<InventoryController>());
            UpdateSlots(2, "1");
            if (bedSlot.transform.childCount > 0)
            {
                Destroy(bedSlot.transform.GetChild(0).gameObject);
            }
        }
    }

    void ClearSlots()
    {
        foreach (GameObject slotObject in slotObjects)
        {
            if (slotObject.transform.childCount > 0)
            {
                for (int i = 0; i < slotObject.transform.childCount; i++)
                {
                    Destroy(slotObject.transform.GetChild(i).gameObject);
                }
            }
        }
    }

    void ResetState()
    {
        ClearSlots();
        inventoryUI.SetActive(false);
        if (bedSlot != null && bedSlot.transform.childCount > 0)
        {
            Destroy(bedSlot.transform.GetChild(0).gameObject);
        }
        bedInventoryUI.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
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
