using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{
    public static float musicVolume;
    public static float fxVolume;

    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeMusicVolumeValue(float value)
    {
        musicVolume = value;
    }

    public void ChangeFXVolumeValue(float value)
    {
        fxVolume = value;
    }


}
