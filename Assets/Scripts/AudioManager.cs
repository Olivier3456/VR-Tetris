using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

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

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DebugLog.Log("There is already an instance of AudioManager.");
        }
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
}
