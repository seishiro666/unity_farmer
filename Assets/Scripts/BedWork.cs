using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Empty,
    SeedPlanted,
    Sprout,
    MaturePlant,
    ReadyToHarvest
}

public class BedWork : MonoBehaviour
{
    public static Action<int, string> onBedClick;

    [SerializeField] GameObject flowerPos;

    State currentState = State.Empty;
    FlowerData flowerData;
    InventoryItem flowerItemData;
    GameObject flowerObj;
    List<GameObject> flowersList = new List<GameObject>();
    bool haveSeed = false;
    GameObject bedSlot;
    GameObject itemPrefab;
    InventoryController inventoryController;

    public void SetupBed()
    {
        if (currentState == State.Empty)
        {
            onBedClick?.Invoke(0, "Посадить");
            OnSetup();
        } else if (currentState == State.MaturePlant)
        {
            onBedClick?.Invoke(1, "Собрать");
        } else if (currentState == State.ReadyToHarvest)
        {
            onBedClick?.Invoke(1, "Собрать");
        }
    }

    void StartGrowth(InventoryItem itemData, GameObject bedSlotUI, 
        GameObject itemInSlotPrefab, InventoryController invController,
        InventorySystem inventorySystem)
    {
        inventoryController = invController;
        flowerItemData = itemData;
        flowerData = flowerItemData.flowerData;
        bedSlot = bedSlotUI;
        itemPrefab = itemInSlotPrefab;

        if (itemData.itemCount >= 4)
        {
            itemData.itemCount -= 4;
            itemData.RefreshItem(itemData.itemCount);

            flowerObj = flowerData.model;

            for (int i = 0; i < 4; i++)
            {
                GameObject tempFlower = Instantiate(flowerObj, flowerPos.transform.GetChild(i));
                flowersList.Add(tempFlower);
            }
            currentState = State.SeedPlanted;

            OnEndSetup();

            StartCoroutine(StartGrowthProcess(itemData.itemCount, inventorySystem));
        }
    }

    IEnumerator StartGrowthProcess(int itemCount, InventorySystem inventorySystem)
    {
        yield return new WaitForSeconds(5f);

        foreach (GameObject flower in flowersList) 
        {
            flower.transform.GetChild(0).gameObject.SetActive(false);
            flower.transform.GetChild(1).gameObject.SetActive(true);
        }
        currentState = State.Sprout;

        yield return new WaitForSeconds(flowerData.seedGrowth);

        foreach (GameObject flower in flowersList)
        {
            flower.transform.GetChild(1).gameObject.SetActive(false);
            flower.transform.GetChild(2).gameObject.SetActive(true);
        }
        currentState = State.MaturePlant;

        haveSeed = true;

        inventoryController.AddItemToBedSlot(inventorySystem, itemCount, true);
    }

    IEnumerator EndGrowth()
    {
        if (!haveSeed)
        {
            yield return new WaitForSeconds(flowerData.growth);

            foreach (GameObject flower in flowersList)
            {
                flower.transform.GetChild(2).gameObject.SetActive(false);
                flower.transform.GetChild(3).gameObject.SetActive(true);
            }
            currentState = State.ReadyToHarvest;
        }
    }

    private void OnSetup()
    {
        InventoryController.onBedBtnClick += StartGrowth;
    }

    private void OnEndSetup()
    {
        InventoryController.onBedBtnClick -= StartGrowth;
    }
}
