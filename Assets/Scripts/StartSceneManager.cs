using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    [Header("Settings")]
    public RectTransform menuRect;
    public TextMeshProUGUI menuTitle;
    public GameObject mainForm;
    public GameObject playForm;
    public GameObject buildForm;
    public Transform playBtnsContent;
    public Transform buildBtnsContent;

    [Header("Loading")]
    public string buildingSceneName = "Building";
    public GameObject[] loadingScenePrefabs;
    public GameObject scenarioButtonPrefab;
    public List<GameObject> playBtnsList;
    public List<GameObject> buildBtnsList;


    void Start()
    {
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();

        ShowForm(1);
    }

    public void ShowForm(int type)
    {
        mainForm.SetActive(type == 1);
        playForm.SetActive(type == 2);
        buildForm.SetActive(type == 3);

        if (type == 1) menuTitle.text = "MENU";
        else if (type == 2) menuTitle.text = "CHẠY TÌNH HUỐNG";
        else if (type == 3) menuTitle.text = "TẠO TÌNH HUỐNG";

        foreach (var item in playBtnsList) Destroy(item.gameObject);
        foreach (var item in buildBtnsList) Destroy(item.gameObject);

        var scDir = Application.dataPath + Helper.Scenarios_DataFolder;
        var files = Directory.GetFiles(scDir, "*.ini");
        int count = 0;
        foreach (var filePath in files)
        {
            count++;
            var f = new IniFile(filePath);
            var name = f.Read(Helper.Sc_Key_ScenarioName, Helper.Sc_Section_General);

            var playBtn = Instantiate(scenarioButtonPrefab, playBtnsContent);
            var buildBtn = Instantiate(scenarioButtonPrefab, buildBtnsContent);

            playBtn.GetComponent<ButtonManagerBasic>().buttonText = count.ToString() + " - " + name;
            playBtn.GetComponent<BtnScenario>().type = BtnScenario.ScenarioLoadType.Play;
            playBtn.GetComponent<BtnScenario>().scenarioName = name;
            playBtn.GetComponent<BtnScenario>().scenarioFilePath = filePath;
            playBtn.GetComponent<BtnScenario>().scenarioFileName = Path.GetFileNameWithoutExtension(filePath);
            playBtn.GetComponent<BtnScenario>().startSceneMng = this;

            buildBtn.GetComponent<ButtonManagerBasic>().buttonText = count.ToString() + " - " + name;
            buildBtn.GetComponent<BtnScenario>().type = BtnScenario.ScenarioLoadType.Build;
            buildBtn.GetComponent<BtnScenario>().scenarioName = name;
            buildBtn.GetComponent<BtnScenario>().scenarioFilePath = filePath;
            buildBtn.GetComponent<BtnScenario>().scenarioFileName = Path.GetFileNameWithoutExtension(filePath);
            buildBtn.GetComponent<BtnScenario>().startSceneMng = this;

            playBtnsList.Add(playBtn);
            buildBtnsList.Add(buildBtn);
        }

        if (type == 2) playForm.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);
        if (type == 3) buildForm.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);

        //menuRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, type == 1 ? 300 : 600);
        if (type == 1)
            menuRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 300);
        else if (type == 2)
            menuRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Clamp(100 * (count + 1), 300, 600));
        else if (type == 3)
            menuRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Clamp(100 * (count + 2), 300, 600));
        else
            menuRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 600);
    }
    

    public void LoadScenario(BtnScenario btn)
    {
        var rm = FindObjectOfType<ResourceManager>();
        rm.file = btn.scenarioFileName;
        rm.mode = btn.type;

        EnableLoadingPrefab();
        StartCoroutine(LoadScene(buildingSceneName));
    }

    void EnableLoadingPrefab()
    {
        if (loadingScenePrefabs.Length > 0)
        {
            int enableIndex = Random.Range(0, loadingScenePrefabs.Length);
            loadingScenePrefabs[enableIndex].SetActive(true);
        }
    }

    IEnumerator LoadScene(string scene)
    {
        yield return new WaitForSeconds(0.1f);

        if (!string.IsNullOrEmpty(scene))
            bl_SceneLoaderUtils.GetLoader.LoadLevel(scene);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
