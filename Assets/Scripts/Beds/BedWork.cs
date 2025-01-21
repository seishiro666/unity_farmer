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
        RndSeedSpawn();
    }

    void CollectSeeds()
    {
        inventoryController.AddItemToInventory(inventorySystem);
    }

    void RndSeedSpawn()
    {
        List<FlowerData> neighboringSeeds = new List<FlowerData>();
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        foreach (var direction in directions)
        {
            BedWork neighboringBed = GetNeighboringBed(direction);

            if (neighboringBed != null && neighboringBed.currentState == State.SeedPlanted)
            {
                neighboringSeeds.Add(neighboringBed.GetFlowerData());
            }
        }

        if (neighboringSeeds.Count > 0)
        {
            bool combineSeeds = UnityEngine.Random.Range(0f, 1f) < 0.5f;

            if (combineSeeds)
            {
                CombineSeeds(neighboringSeeds);
            }
            else
            {
                int randomSeedIndex = UnityEngine.Random.Range(0, neighboringSeeds.Count);
                InventorySystem seedInventorySystem = new InventorySystem
                {
                    item = neighboringSeeds[randomSeedIndex],
                    count = 4,
                    isSeed = true
                };

                inventorySystem = seedInventorySystem;
            }
        }
        else
        {
            int seedNum = 0;
            InventorySystem seedInventorySystem = new InventorySystem
            {
                item = flowerData.potentialSeeds[seedNum],
                count = 4,
                isSeed = true
            };

            inventorySystem = seedInventorySystem;
        }
    }

    BedWork GetNeighboringBed(Vector2 direction)
    {
        Vector3 neighborPosition = transform.position + new Vector3(direction.x, 0, direction.y);

        BedWork[] allBeds = FindObjectsOfType<BedWork>();

        foreach (var bed in allBeds)
        {
            if (Vector3.Distance(neighborPosition, bed.transform.position) < 5f)
            {
                return bed;
            }
        }

        return null;
    }

    void CombineSeeds(List<FlowerData> neighboringSeeds)
    {
        int randomNeighborIndex = UnityEngine.Random.Range(0, neighboringSeeds.Count);
        FlowerData selectedSeed = neighboringSeeds[randomNeighborIndex];

        string currentFlowerName = flowerData.name.Replace("Flower", "");
        string neighborFlowerName = selectedSeed.name.Replace("Flower", "");

        FlowerData combinedSeed = flowerData.potentialSeeds.Find(seed =>
            seed.name.Contains(currentFlowerName) && seed.name.Contains(neighborFlowerName));

        if (combinedSeed != null)
        {
            InventorySystem combinedSeedSystem = new InventorySystem
            {
                item = combinedSeed,
                count = 4,
                isSeed = true
            };

            inventorySystem = combinedSeedSystem;
        }
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
