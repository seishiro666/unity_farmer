using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [Header("Script")]
    [SerializeField] BedController bedController;

    [Header("Inventory")]
    [SerializeField] GameObject inventoryUI;
    public Inventory inventory;
    [SerializeField] List<GameObject> slotObjects;
    [SerializeField] GameObject itemPrefab;
    int MaxStackSize = 32;
    Transform bedInventoryUI;

    [Header("Player")]
    public PlayerController playerController;
    [SerializeField] UserData userData;

    [Header("Shop")]
    [SerializeField] GameObject shopUI;
    [SerializeField] List<GameObject> shopObjects;
    [SerializeField] List<FlowerData> shopData;
    [SerializeField] GameObject shopItemPrefab;

    [Header("Sell/Delete")]
    [SerializeField] GameObject sellSlot;
    [SerializeField] GameObject deleteSlot;
    [SerializeField] GameObject bedSlot;

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
            playerController.AddMoney(Mathf.CeilToInt(itemData.item.price * (itemData.count / 4)));
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

    void UpdateSlots(int typeOfItems = 1, string actionBtnText = "1")
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

        bedInventoryUI.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = actionBtnText;

        if (typeOfItems == 0)
        {
            inventoryUI.transform.localPosition = new Vector3(200, 0, 0);
            bedInventoryUI.gameObject.SetActive(true);
            bedInventoryUI.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            bedInventoryUI.GetChild(1).GetComponent<Button>().onClick.AddListener( delegate { SubToBedBtnEvent(itemData); } );
        }
        else
        {
            inventoryUI.transform.localPosition = Vector3.zero;
            bedInventoryUI.gameObject.SetActive(false);
        }
    }

    public void UpdateInventoryFromBtn()
    {
        UpdateSlots();
    }

    public void AddItemToInventory(InventorySystem newItem)
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

    void SubToBedBtnEvent(InventorySystem itemInventoryData)
    {
        if (bedSlot.transform.childCount > 0 && bedSlot.transform.GetChild(0).GetComponent<InventoryItem>().itemCount >= 4)
        {
            onBedBtnClick?.Invoke(bedSlot.transform.GetChild(0).GetComponent<InventoryItem>(), bedSlot, gameObject.GetComponent<InventoryController>());
            UpdateSlots();
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

    public void CollectItemsFromShop()
    {
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

            tempInvItem.SetupSlot(tempInvSystem.item.seedIcon, tempInvSystem.count, tempInvSystem);
            shopObjects[i].GetComponent<Button>().onClick.AddListener( delegate { BuyItemFromShop(tempInvSystem); } );
        }

        shopUI.SetActive(true);
    }

    void BuyItemFromShop(InventorySystem tempInvSystem)
    {
        if (playerController.money >= tempInvSystem.item.price)
        {
            tempInvSystem.count = 4;
            playerController.AddMoney(-tempInvSystem.item.price);
            AddItemToInventory(tempInvSystem);
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
