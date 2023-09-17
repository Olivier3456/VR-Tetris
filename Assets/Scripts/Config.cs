using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{
    public static float musicVolume;
    public static float fxVolume;

    public AudioSource musicAudioSource;
    public AudioSource fxAudioSource;


    public void ChangeMusicVolumeValue(float value)
    {
        musicVolume = value;

        if (musicAudioSource != null)
        {
            fxAudioSource.volume = musicVolume;
        }
    }

    public void ChangeFXVolumeValue(float value)
    {
        fxVolume = value;

        if (fxAudioSource != null)
        {
            fxAudioSource.volume = fxVolume;
        }
    }
}
