using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnScenario : MonoBehaviour
{
    public enum ScenarioLoadType
    {
        Play = 1,
        Build = 2
    }

    public ScenarioLoadType type;
    public string scenarioName;
    public string scenarioFilePath;
    public string scenarioFileName;
    public StartSceneManager startSceneMng;

    public void LoadScenario()
    {
        startSceneMng.LoadScenario(this);
    }
}


