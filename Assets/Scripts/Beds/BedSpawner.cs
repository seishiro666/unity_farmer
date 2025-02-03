using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Класс, отвечающий за спавн кроватей в игре
public class BedSpawner : MonoBehaviour
{
    [SerializeField] GameObject bedPrefab; // Префаб для кровати
    [SerializeField] GameObject bedsGrid; // Родительский объект для кроватей
    [SerializeField] UserData userData; // Данные пользователя

    float spacingColumn = -4.5f; // Расстояние между колонками
    float spacingRows = 5f; // Расстояние между рядами
    int rows, cols; // Количество рядов и колонок

    // Метод для начала спавна кроватей
    public void StartSpawn()
    {
        cols = 3; // Устанавливаем количество колонок
        rows = 2; // Устанавливаем количество рядов

        SetupBedCount(userData.lvl); // Настраиваем количество кроватей в зависимости от уровня
    }

    // Метод для спавна кроватей
    public void SpawnBeds(int rows, int columns)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                // Вычисляем позицию для спавна кровати
                Vector3 position = new Vector3(
                    column * spacingColumn,
                    0.16f,
                    row * spacingRows
                );

                // Создаем экземпляр кровати
                GameObject bed = Instantiate(bedPrefab, position, Quaternion.identity, bedsGrid.transform);
                bed.transform.localPosition = position; // Устанавливаем локальную позицию
            }
        }
    }

    // Метод для настройки количества кроватей в зависимости от уровня
    public void SetupBedCount(int lvl)
    {
        cols = 3; // Устанавливаем количество колонок
        rows = 2; // Устанавливаем количество рядов

        // Если уровень больше 1 и меньше 5, добавляем дополнительные кровати
        if (lvl > 1 && lvl < 5)
        {
            int additionalBeds = (lvl - 1) * 3; // Вычисляем количество дополнительных кроватей
            rows = Mathf.CeilToInt((float)(6 + additionalBeds) / cols); // Пересчитываем количество рядов
        }

        SpawnBeds(rows, cols); // Спавним кровати
    }
}