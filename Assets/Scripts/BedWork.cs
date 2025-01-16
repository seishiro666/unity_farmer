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
    GameObject flowerObj;
    List<GameObject> flowersList = new List<GameObject>();
    bool haveSeed = false;

    public void SetupBed()
    {
        if (currentState == State.Empty)
        {
            onBedClick?.Invoke(0, "��������");
            OnSetup();
        } else if (currentState == State.MaturePlant)
        {
            onBedClick?.Invoke(1, "�������");
        } else if (currentState == State.ReadyToHarvest)
        {
            onBedClick?.Invoke(1, "�������");
        }
    }

    void StartGrowth(InventoryItem itemData)
    {
        flowerData = itemData.flowerData;
        
        flowerObj = flowerData.model;
        
        for (int i = 0; i < 4; i++)
        {
            GameObject tempFlower = Instantiate(flowerObj, flowerPos.transform.GetChild(i));
            flowersList.Add(tempFlower);
        }
        currentState = State.SeedPlanted;

        OnEndSetup();

        StartCoroutine(StartGrowthProcess());
    }

    IEnumerator StartGrowthProcess()
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
