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
    [Header ("Inventory")]
    [SerializeField] GameObject inventoryUI;
    public Inventory inventory;
    [SerializeField] List<GameObject> slotObjects;
    [SerializeField] GameObject bedSlot;
    [SerializeField] GameObject sellSlot;
    [SerializeField] GameObject deleteSlot;
    [SerializeField] GameObject itemPrefab;
    int MaxStackSize = 32;
    Transform bedInventoryUI;

    [Header("Shop")]
    [SerializeField] GameObject shopUI;
    [SerializeField] List<GameObject> shopObjects;
    [SerializeField] List<FlowerData> shopData;
    [SerializeField] GameObject shopItemPrefab;

    [Header("Player")]
    [SerializeField] PlayerController playerController;
    [SerializeField] UserData userData;

    public static Action<InventoryItem, GameObject, InventoryController> onBedBtnClick;

    private void Awake()
    {
        bedInventoryUI = inventoryUI.transform.GetChild(3);
    }

    void Update()
    {
        HandleSellSlot();
        HandleDeleteSlot();
    }

    void HandleSellSlot()
    {
        if (sellSlot.transform.childCount > 0)
        {
            InventoryItem itemToSell = sellSlot.transform.GetChild(0).GetComponent<InventoryItem>();
            InventorySystem itemData = itemToSell.GetInventorySystemData();
            Destroy(sellSlot.transform.GetChild(0).gameObject);
            inventory.inventorySystem.Remove(itemData);
            playerController.AddMoney(itemData.item.price);
        }
    }

    void HandleDeleteSlot()
    {
        if (deleteSlot.transform.childCount > 0)
        {
            InventoryItem itemToDelete = deleteSlot.transform.GetChild(0).GetComponent<InventoryItem>();
            InventorySystem itemData = itemToDelete.GetInventorySystemData();
            Destroy(deleteSlot.transform.GetChild(0).gameObject);
            inventory.inventorySystem.Remove(itemData);
        }
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
                }
                else if (typeOfItems != 0)
                {
                    GameObject tempItem = Instantiate(itemPrefab, slot.transform);
                    InventoryItem inventoryItem = tempItem.GetComponent<InventoryItem>();
                    Sprite itemIcon = itemData.isSeed ? itemData.item.seedIcon : itemData.item.icon;
                    inventoryItem.SetupSlot(itemIcon, itemData.count, itemData);
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
        AddItemToInventory(invSystem);
        Destroy(bedSlot.transform.GetChild(0).gameObject);
        UpdateSlots(2, "1");

        if (!isEnd)
        {
            curBed.SwapState();
        } else
        {
            float exp = invSystem.item.expReward;
            playerController.AddExperience(exp);
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

    public void SetupShopSlots()
    {
        shopUI.SetActive(true);

        for (int i = 0; i < shopObjects.Count; i++)
        {
            if (shopObjects[i].transform.childCount != 0)
            {
                Destroy(shopObjects[i].transform.GetChild(0).gameObject);
            }

            GameObject tempItem = Instantiate(shopItemPrefab, shopObjects[i].transform);
            InventoryItem tempInvItem = tempItem.GetComponent<InventoryItem>();
            tempInvItem.flowerData = shopData[i];
            InventorySystem tempInvSystem = new InventorySystem();
            tempInvSystem.item = shopData[i];
            tempInvSystem.isSeed = false;
            tempInvSystem.count = 50;
            tempInvItem.SetupSlot(tempInvSystem.item.icon, tempInvSystem.count, tempInvSystem);
            shopObjects[i].GetComponent<Button>().onClick.RemoveAllListeners();
            shopObjects[i].GetComponent<Button>().onClick.AddListener( delegate { BuyItemFromShop(tempInvSystem); });
        }
    }

    void BuyItemFromShop(InventorySystem tempInvSystem)
    {
        tempInvSystem.count = 4;
        playerController.AddMoney(-tempInvSystem.item.price);
        AddItemToInventory(tempInvSystem);
    }

    void AddItemToInventory(InventorySystem newItem)
    {
        bool itemAdded = false;

        foreach (var existingItem in inventory.inventorySystem)
        {
            if (existingItem.item == newItem.item && existingItem.isSeed == newItem.isSeed)
            {
                int availableSpace = MaxStackSize - existingItem.count;

                if (availableSpace > 0)
                {
                    int amountToAdd = Mathf.Min(newItem.count, availableSpace);
                    existingItem.count += amountToAdd;
                    newItem.count -= amountToAdd;

                    if (newItem.count <= 0)
                    {
                        itemAdded = true;
                        break;
                    }
                }
            }
        }

        while (newItem.count > 0 && !itemAdded)
        {
            InventorySystem newStack = new InventorySystem
            {
                item = newItem.item,
                count = Mathf.Min(newItem.count, MaxStackSize),
                isSeed = newItem.isSeed
            };

            newItem.count -= newStack.count;
            inventory.inventorySystem.Add(newStack);
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
