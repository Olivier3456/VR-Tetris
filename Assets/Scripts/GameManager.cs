using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [HideInInspector] public bool gameOver = false;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;            
        }
        else
        {
            DebugLog.Log("An instance of GameManager already exists!");
        }
    }
}
