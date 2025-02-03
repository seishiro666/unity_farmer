using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Класс, отвечающий за управление игроком
public class PlayerController : MonoBehaviour
{
    [Header("Скрипты")]
    [SerializeField] UserData userData; // Данные пользователя
    [SerializeField] BedSpawner bedSpawner; // Спавнер кроватей
    [SerializeField] SoundController soundController; // Контроллер звука
    [SerializeField] InventoryController inventoryController; // Контроллер инвентаря

    [Header("UI")]
    [SerializeField] BtnSettingController soundBtnScript; // Скрипт для управления звуком
    [SerializeField] BtnSettingController musicBtnScript; // Скрипт для управления музыкой
    [SerializeField] Slider soundSlider; // Слайдер для звука
    [SerializeField] Slider musicSlider; // Слайдер для музыки
    [SerializeField] TMP_Text moneyText; // Текст для отображения денег
    [SerializeField] Slider lvlSlider; // Слайдер для уровня
    [SerializeField] TMP_Text userLvlText; // Текст для отображения уровня пользователя

    [Header("Дополнительно")]
    [SerializeField] ShopConfig shopConfig; // Конфигурация магазина

    public float lvlProgress; // Прогресс уровня
    public int money, lvl; // Деньги и уровень игрока

    private void Awake()
    {
        LoadData(); // Загружаем данные
        UpdateUserUI(); // Обновляем UI пользователя
    }

    // Метод для сохранения данных
    public void SaveData()
    {
        userData.musicOn = musicBtnScript.isOn; // Сохраняем состояние музыки
        userData.soundOn = soundBtnScript.isOn; // Сохраняем состояние звука
        userData.musicValue = musicSlider.value; // Сохраняем значение музыки
        userData.soundValue = soundSlider.value; // Сохраняем значение звука
        userData.money = money; // Сохраняем деньги
        userData.lvl = lvl; // Сохраняем уровень
        userData.lvlProgress = lvlProgress; // Сохраняем прогресс уровня

        CheckSoundAndMusic(); // Проверяем состояние звука и музыки
    }

    // Метод для загрузки данных
    private void LoadData()
    {
        lvlProgress = userData.lvlProgress; // Загружаем прогресс уровня
        money = userData.money; // Загружаем деньги
        lvl = userData.lvl; // Загружаем уровень

        soundBtnScript.isOn = userData.soundOn; // Устанавливаем состояние звука
        musicBtnScript.isOn = userData.musicOn; // Устанавливаем состояние музыки
        soundBtnScript.UpdateState(); // Обновляем состояние звука
        musicBtnScript.UpdateState(); // Обновляем состояние музыки

        CheckItemsForShop(); // Проверяем предметы для магазина
        CheckSoundAndMusic(); // Проверяем состояние звука и музыки
    }

    // Метод для проверки предметов в магазине
    void CheckItemsForShop()
    {
        if (lvl >= 1 && lvl < 5)
        {
            List<FlowerData> newItemInShop = new List<FlowerData>(); // Список новых предметов в магазине

            if (lvl >= 1)
            {
                newItemInShop.Add(shopConfig.shopConfigData[0]); // Добавляем предметы в магазин
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
                inventoryController.shopData.Add(item); // Добавляем предметы в магазин
            }
        }
    }

    // Метод для обновления UI пользователя
    private void UpdateUserUI()
    {
        soundSlider.value = userData.soundValue; // Устанавливаем значение слайдера звука
        musicSlider.value = userData.musicValue; // Устанавливаем значение слайдера музыки
        moneyText.text = userData.money.ToString(); // Обновляем текст с деньгами
        lvlSlider.value = userData.lvlProgress; // Устанавливаем значение слайдера прогресса уровня
        userLvlText.text = userData.lvl.ToString(); // Обновляем текст с уровнем пользователя
    }

    // Метод для проверки состояния звука и музыки
    void CheckSoundAndMusic()
    {
        if (userData.soundOn)
        {
            soundController.SetupSoundVolume(userData.soundValue); // Устанавливаем громкость звука
        }

        if (userData.musicOn)
        {
            soundController.SetupMusicVolume(userData.musicValue); // Устанавливаем громкость музыки
            soundController.StartMusic(); // Запускаем музыку
        }
        else
        {
            soundController.StopMusic(); // Останавливаем музыку
        }
    }

    // Метод для добавления опыта
    public void AddExperience(float amount)
    {
        lvlProgress += amount; // Увеличиваем прогресс уровня
        if (lvlProgress >= 1f)
        {
            lvlProgress -= 1f; // Сбрасываем прогресс
            lvl++; // Увеличиваем уровень
            bedSpawner.SetupBedCount(lvl); // Настраиваем количество кроватей
        }
        SaveData(); // Сохраняем данные
        UpdateUserUI(); // Обновляем UI пользователя
    }

    // Метод для добавления денег
    public void AddMoney(int amount)
    {
        money += amount; // Увеличиваем количество денег
        SaveData(); // Сохраняем данные
        UpdateUserUI(); // Обновляем UI пользователя
    }
}