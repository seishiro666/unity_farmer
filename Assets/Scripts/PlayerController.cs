using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header ("Scripts")]
    [SerializeField] UserData userData;
    [SerializeField] BedSpawner bedSpawner;
    [SerializeField] SoundController soundController;
    [SerializeField] InventoryController inventoryController;

    [Header ("UI")]
    [SerializeField] BtnSettingController soundBtnScript;
    [SerializeField] BtnSettingController musicBtnScript;
    [SerializeField] Slider soundSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] TMP_Text moneyText;
    [SerializeField] Slider lvlSlider;
    [SerializeField] TMP_Text userLvlText;

    [Header("Extra")]
    [SerializeField] ShopConfig shopConfig;

    public float lvlProgress;
    public int money, lvl;

    private void Awake()
    {
        LoadData();
        UpdateUserUI();
    }

    public void SaveData()
    {
        userData.musicOn = musicBtnScript.isOn;
        userData.soundOn = soundBtnScript.isOn;
        userData.musicValue = musicSlider.value;
        userData.soundValue = soundSlider.value;
        userData.money = money;
        userData.lvl = lvl;
        userData.lvlProgress = lvlProgress;

        CheckSoundAndMusic();
    }

    private void LoadData()
    {
        lvlProgress = userData.lvlProgress;
        money = userData.money;
        lvl = userData.lvl;

        soundBtnScript.isOn = userData.soundOn;
        musicBtnScript.isOn = userData.musicOn;
        soundBtnScript.UpdateState();
        musicBtnScript.UpdateState();

        CheckItemsForShop();
        CheckSoundAndMusic();
    }

    void CheckItemsForShop()
    {
        if (lvl >= 1 && lvl < 5)
        {
            List<FlowerData> newItemInShop = new List<FlowerData>();

            if (lvl >= 1)
            {
                newItemInShop.Add(shopConfig.shopConfigData[0]);
                newItemInShop.Add(shopConfig.shopConfigData[1]);
                newItemInShop.Add(shopConfig.shopConfigData[2]);
            }

            switch (lvl)
            {
                case 2:
                    newItemInShop.Add(shopConfig.shopConfigData[3]);
                    break;
                case 3:
                    newItemInShop.Add(shopConfig.shopConfigData[3]);
                    newItemInShop.Add(shopConfig.shopConfigData[4]);
                    break;
                case 4:
                    newItemInShop.Add(shopConfig.shopConfigData[3]);
                    newItemInShop.Add(shopConfig.shopConfigData[4]);
                    newItemInShop.Add(shopConfig.shopConfigData[5]);
                    break;
            }

            foreach (FlowerData item in newItemInShop)
            {
                inventoryController.shopData.Add(item);
            }
        }
    }

    private void UpdateUserUI()
    {
        soundSlider.value = userData.soundValue;
        musicSlider.value = userData.musicValue;
        moneyText.text = userData.money.ToString();
        lvlSlider.value = userData.lvlProgress;
        userLvlText.text = userData.lvl.ToString();
    }

    void CheckSoundAndMusic()
    {
        if (userData.soundOn)
        {
            soundController.SetupSoundVolume(userData.soundValue);
        }

        if (userData.musicOn)
        {
            soundController.SetupMusicVolume(userData.musicValue);
            soundController.StartMusic();
        } else
        {
            soundController.StopMusic();
        }
    }

    public void AddExperience(float amount)
    {
        lvlProgress += amount;
        if (lvlProgress >= 1f)
        {
            lvlProgress -= 1f;
            lvl++;
            bedSpawner.SetupBedCount(lvl);
        }
        SaveData();
        UpdateUserUI();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        SaveData(); 
        UpdateUserUI();
    }
}
