using UnityEngine;

// Класс, отвечающий за управление настройками кнопок
public class BtnSettingController : MonoBehaviour
{
    [SerializeField] GameObject settingOn; // Объект для включенных настроек
    [SerializeField] GameObject settingOff; // Объект для выключенных настроек
    [SerializeField] PlayerController controller; // Контроллер игрока

    public bool isOn = true; // Состояние настройки

    private void Start()
    {
        UpdateState(); // Обновляем состояние при старте
    }

    // Метод для переключения настроек
    public void SwapSettings()
    {
        isOn = !isOn; // Переключаем состояние
        controller.SaveData(); // Сохраняем данные
        UpdateState(); // Обновляем состояние
    }

    // Метод для обновления состояния
    public void UpdateState()
    {
        settingOn.SetActive(isOn); // Активируем или деактивируем объект для включенных настроек
        settingOff.SetActive(!isOn); // Активируем или деактивируем объект для выключенных настроек
    }
}