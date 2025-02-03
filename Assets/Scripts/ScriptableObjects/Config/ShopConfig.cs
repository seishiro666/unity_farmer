using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Класс, отвечающий за конфигурацию магазина
[CreateAssetMenu(fileName = "NewConfig", menuName = "ScriptableObject/Config")]
public class ShopConfig : ScriptableObject
{
    public List<FlowerData> shopConfigData; // Данные о цветах в магазине
}