using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicAudioSource;
    public AudioClip musicAudioClip;
    [Space(10)]
    public AudioSource fxAudioSource;
    public AudioClip pieceDroppedErrorClip;
    public AudioClip pieceDroppedOkClip;
    public AudioClip fullFloorClip;
    public AudioClip pieceGroundedClip;
    public AudioClip uiClickClip;

    public static AudioManager instance;
   
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("An instance of AudioManager already exists!");
        }
    }


    private void Start()
    {
        musicAudioSource.volume = Config.musicVolume;
        fxAudioSource.volume = Config.fxVolume;
    }


    public void Play_PieceDroppedError()
    {
        fxAudioSource.PlayOneShot(pieceDroppedErrorClip);
    }

    public void Play_PieceDroppedGood()
    {
        fxAudioSource.PlayOneShot(pieceDroppedOkClip);
    }

    public void Play_FullFloor()
    {
        fxAudioSource.PlayOneShot(fullFloorClip);
    }

    public void Play_PieceGrounded()
    {
        fxAudioSource.PlayOneShot(pieceGroundedClip);
    }

    public void PlayUIClickSound()
    {
        fxAudioSource.PlayOneShot(uiClickClip);
    }

    public void IncreaseMusicPitch()
    {
        musicAudioSource.pitch += 0.01f;
    }
}
