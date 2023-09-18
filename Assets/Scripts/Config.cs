using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{
    public static float musicVolume = 1;
    public static float fxVolume = 1;

    public AudioSource musicAudioSource;
    public AudioSource fxAudioSource;

    private bool firstInstance = true;

    private void Start()
    {
        if (!firstInstance)
        {
            Destroy(gameObject);
            return;
        }

        firstInstance = false;
        DontDestroyOnLoad(this);

        if (musicAudioSource != null)
        {
            fxAudioSource.volume = musicVolume;
        }

        if (fxAudioSource != null)
        {
            fxAudioSource.volume = fxVolume;
        }
    }



    public void ChangeMusicVolumeValue(float value)
    {
        musicVolume = value;

        if (musicAudioSource != null)
        {
            musicAudioSource.volume = musicVolume;
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
