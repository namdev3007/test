using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public bl_SceneLoader loader;

    void Start()
    {
        AudioListener.volume = 1;
    }


    public void ResetScenario()
    {
        loader.gameObject.SetActive(true);
        StartCoroutine(LoadBuildingScene());
    }

    IEnumerator LoadBuildingScene()
    {
        AudioListener.volume = 0;

        yield return new WaitForSeconds(0.1f);

        bl_SceneLoaderUtils.GetLoader.LoadLevel("Building");
    }

    public void BackToMainScene()
    {
        loader.gameObject.SetActive(true);
        StartCoroutine(LoadStartScene());
    }

    IEnumerator LoadStartScene()
    {
        AudioListener.volume = 0;

        yield return new WaitForSeconds(0.1f);

        bl_SceneLoaderUtils.GetLoader.LoadLevel("Start");
    }
}
