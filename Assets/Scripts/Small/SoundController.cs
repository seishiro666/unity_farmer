using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] AudioSource mainMusic;
    [SerializeField] AudioClip mainMusicClip;

    [SerializeField] AudioSource mainAudio;

    public void StartMusic()
    {
        mainMusic.clip = mainMusicClip;
        mainMusic.Play();
    }

    public void StopMusic()
    {
        mainMusic.Pause();
    }

    public void SetupMusicVolume(float volume)
    {
        mainMusic.volume = volume;
    }

    public void StartSound(AudioClip mainAudioClip)
    {
        mainAudio.clip = mainAudioClip;
        mainAudio.Play();
    }

    public void StopSound()
    {
        mainAudio.Stop();
    }

    public void SetupSoundVolume(float volume)
    {
        mainAudio.volume = volume;
    }
}
