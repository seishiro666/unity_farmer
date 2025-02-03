using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Класс, отвечающий за данные пользователя
[CreateAssetMenu(fileName = "User Data", menuName = "ScriptableObject/UserData")]
public class UserData : ScriptableObject
{
    public bool musicOn = true; // Включена ли музыка
    public bool soundOn = true; // Включен ли звук
    public float musicValue = 0.5f; // Значение громкости музыки
    public float soundValue = 0.5f; // Значение громкости звука
    public int money = 150; // Количество денег
    public int lvl = 1; // Уровень пользователя
    public float lvlProgress = 0f; // Прогресс уровня
}