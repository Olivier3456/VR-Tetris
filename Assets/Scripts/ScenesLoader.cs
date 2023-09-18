using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesLoader : MonoBehaviour
{
    public void LoadLevelScene()
    {
        SceneManager.LoadSceneAsync(1);            
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
