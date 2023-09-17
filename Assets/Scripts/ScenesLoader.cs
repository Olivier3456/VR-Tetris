using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesLoader : MonoBehaviour
{
    public void LoadLevelScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetSceneByBuildIndex(1).name);
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetSceneByBuildIndex(0).name);
    }
}
