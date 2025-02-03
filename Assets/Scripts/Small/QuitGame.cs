using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Класс, отвечающий за выход из игры
public class QuitGame : MonoBehaviour
{
    // Метод для выхода из игры
    public void Quit()
    {
        Application.Quit(); // Закрываем приложение
    }
}