using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Класс, отвечающий за управление звуком
public class SoundController : MonoBehaviour
{
    [SerializeField] AudioSource mainMusic; // Основной источник музыки
    [SerializeField] AudioClip mainMusicClip; // Основная музыкальная дорожка

    [SerializeField] AudioSource mainAudio; // Основной источник звука

    // Метод для запуска музыки
    public void StartMusic()
    {
        mainMusic.clip = mainMusicClip; // Устанавливаем музыкальную дорожку
        mainMusic.Play(); // Запускаем музыку
    }

    // Метод для остановки музыки
    public void StopMusic()
    {
        mainMusic.Pause(); // Приостанавливаем музыку
    }

    // Метод для настройки громкости музыки
    public void SetupMusicVolume(float volume)
    {
        mainMusic.volume = volume; // Устанавливаем громкость музыки
    }

    // Метод для запуска звука
    public void StartSound(AudioClip mainAudioClip)
    {
        mainAudio.clip = mainAudioClip; // Устанавливаем звуковую дорожку
        mainAudio.Play(); // Запускаем звук
    }

    // Метод для остановки звука
    public void StopSound()
    {
        mainAudio.Stop(); // Останавливаем звук
    }

    // Метод для настройки громкости звука
    public void SetupSoundVolume(float volume)
    {
        mainAudio.volume = volume; // Устанавливаем громкость звука
    }
}