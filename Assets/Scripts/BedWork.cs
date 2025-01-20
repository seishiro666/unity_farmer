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
    Harvest,
    End
}

public class BedWork : MonoBehaviour
{
    public static Action<int, string> onBedClick;

    [SerializeField] GameObject flowerPos;

    [SerializeField] State currentState = State.Empty;
    FlowerData flowerData;
    InventoryItem flowerItemData;
    GameObject flowerObj;
    List<GameObject> flowersList = new List<GameObject>();
    [SerializeField] bool haveSeed = false;
    GameObject bedSlot;
    GameObject itemPrefab;
    InventoryController inventoryController;
    InventorySystem inventorySystem;

    public void SetupBed()
    {
        if (currentState == State.Empty)
        {
            onBedClick?.Invoke(0, "Посадить");
            OnSetup();
        } else if (currentState == State.MaturePlant)
        {
            CollectSeeds();
            inventorySystem = null;
            SwapState();
        } else if (currentState == State.End)
        {
            CollectFlowers();
            ClearBed();
        }
    }

    void StartGrowth(InventoryItem itemData, GameObject bedSlotUI, InventoryController invController)
    {
        inventoryController = invController;
        flowerItemData = itemData;
        flowerData = flowerItemData.flowerData;
        bedSlot = bedSlotUI;

        if (itemData.itemCount >= 4)
        {
            itemData.itemCount -= 4;

            if (itemData.itemCount <= 0)
            {
                inventoryController.inventory.inventorySystem.Remove(itemData.GetInventorySystemData());
                itemData.ClearSlot();
            } else
            {
                itemData.RefreshItem(itemData.itemCount);
            }

            flowerObj = flowerData.model;

            for (int i = 0; i < 4; i++)
            {
                GameObject tempFlower = Instantiate(flowerObj, flowerPos.transform.GetChild(i));
                flowersList.Add(tempFlower);
            }
            currentState = State.SeedPlanted;

            OnEndSetup();

            StartCoroutine(StartGrowthProcess(itemData));
        }
    }

    IEnumerator StartGrowthProcess(InventoryItem itemData)
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
        FlowerData seedData = itemData.flowerData.potentialSeeds[RndSeedSpawn(itemData)];
        InventorySystem seedInventorySystem = new InventorySystem();
        seedInventorySystem.item = seedData;
        seedInventorySystem.count = 4;
        seedInventorySystem.isSeed = true;
        inventorySystem = seedInventorySystem;
    }

    void CollectSeeds()
    {
        inventoryController.AddItemToInventory(inventorySystem);
    }

    int RndSeedSpawn(InventoryItem itemData)
    {
        int seedNum = -1;

        if (itemData.flowerData.potentialSeeds.Count > 1)
        {
            int rndSeed = UnityEngine.Random.Range(1, 11);

            if (rndSeed >= 1 && rndSeed <= 6)
            {
                seedNum = 0;
            }
            else if (rndSeed >= 7 && rndSeed <= 8)
            {
                seedNum = 1;
            }
            else if (rndSeed >= 9 && rndSeed <= 10)
            {
                seedNum = 2;
            }
        }
        else
        {
            seedNum = 0;
        }

        return seedNum;
    }

    void SwapState()
    {
        haveSeed = false;
        StartCoroutine(EndGrowth());
    }

    IEnumerator EndGrowth()
    {
        currentState = State.Harvest;

        if (!haveSeed)
        {
            yield return new WaitForSeconds(flowerData.growth);

            foreach (GameObject flower in flowersList)
            {
                flower.transform.GetChild(2).gameObject.SetActive(false);
                flower.transform.GetChild(3).gameObject.SetActive(true);
            }
            currentState = State.End;

            InventorySystem seedInventorySystem = new InventorySystem();
            seedInventorySystem.item = flowerData;
            seedInventorySystem.count = 4;
            seedInventorySystem.isSeed = false;
            inventorySystem = seedInventorySystem;
        }
    }

    void CollectFlowers()
    {
        inventoryController.playerController.AddExperience(inventorySystem.item.expReward);
        inventoryController.AddItemToInventory(inventorySystem);
    }

    void ClearBed()
    {
        StopAllCoroutines();

        currentState = State.Empty;
        haveSeed = false;
        inventorySystem = null;

        foreach (Transform flower in flowerPos.transform)
        {
            foreach (Transform child in flower)
            {
                Destroy(child.gameObject);
            }
        }
        flowersList.Clear();

        if (bedSlot != null && bedSlot.transform.childCount > 0)
        {
            Destroy(bedSlot.transform.GetChild(0).gameObject);
        }

        flowerData = null;
        flowerItemData = null;
        flowerObj = null;

        OnEndSetup();
    }

    public FlowerData GetFlowerData()
    {
        return flowerData;
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
