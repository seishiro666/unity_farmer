using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Класс, отвечающий за управление кроватью
public class BedController : MonoBehaviour
{
    GameObject currentBed; // Текущая кровать

    // Метод для настройки кровати
    public void SetupBed(GameObject bed)
    {
        currentBed = bed; // Устанавливаем текущую кровать
        BedWork bedWork = currentBed.transform.parent.GetComponent<BedWork>(); // Получаем компонент BedWork
        bedWork.SetupBed(); // Настраиваем кровать
    }

    // Метод для получения текущей кровати
    public GameObject GetCurrentBed()
    {
        return currentBed; // Возвращаем текущую кровать
    }

    // Метод для очистки текущей кровати
    public void ClearBed()
    {
        currentBed = null; // Очищаем текущую кровать
    }
}