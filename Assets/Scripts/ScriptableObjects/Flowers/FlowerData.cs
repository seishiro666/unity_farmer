using System.Collections.Generic;
using UnityEngine;

// Класс, отвечающий за данные о цветах
[CreateAssetMenu(fileName = "NewFlower", menuName = "ScriptableObject/Flower")]
public class FlowerData : ScriptableObject
{
    public string flowerName; // Название цветка
    public Sprite icon; // Иконка цветка
    public Sprite seedIcon; // Иконка семени
    public GameObject model; // Модель цветка
    public float seedGrowth; // Время роста семени
    public float growth; // Время роста цветка
    public List<FlowerData> potentialSeeds; // Потенциальные семена
    public int price; // Цена цветка
    public float expReward; // Награда за опыт
}