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
    public static Action<bool, string> onBedClick;

    [SerializeField] GameObject flowerPos;
    State currentState = State.Empty;

    public void SetupBed()
    {
        if (currentState == State.Empty)
        {
            onBedClick?.Invoke(true, "Посадить");
            OnSetup();
        }
    }

    void StartGrowth()
    {
        OnEndSetup();
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
