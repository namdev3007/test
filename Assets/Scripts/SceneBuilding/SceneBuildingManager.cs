using SWS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UIWidgets;
using UIWidgets.Examples;
using UnityEngine;
using UnityEngine.UI;

public class SceneBuildingManager : MonoBehaviour
{
    public ResourceManager resourceMgr;
    [Space(10)]
    public Camera mainCamera;
    [Space(5)]
    public BtnScenario.ScenarioLoadType sceneMode;
    public GameObject menuButton;
    public GameObject mapButton;
    public bool hideMapButton;
    public GameObject buildingMenu;
    float buildingMenuScaleFactor = 1;
    public GameObject playingMenu;
    public RectTransform scenarioMenuAnchor;
    

    [Space(10)]
    public GameObject locationPopup;

    [Space(10)]
    public Dialog yesNoDialog;
    public Dialog infoDialog;
    public GameObject modalHelper;

    [Space(5)]
    public bl_SceneLoader loader;
    CreatePathType currentCreatePathType;
    [HideInInspector]
    public bool isCreatingPath;

    [Header("Music select")]
    public FileDialog musicSelectFileDialog;
    public string selectedFileName;
    public string selectedFileFullPath;
    public int currentSoundItemIndex;
    public ShowMusicSelectDialogType selectMusicType;

    [Header("Camera")]
    public CameraControl camControl;
    public splineMove camPathSpLine;
    public GameObject camHaveFollowObjIcon;
    public GameObject camNotHaveFollowObjIcon;
    public RawImage camFollowObjPrefabIcon;
    public Text camFollowObjName;
    public SpinnerFloat followedObjDistanceSpinner;
    public SpinnerFloat followedObjHeightSpinner;
    public SpinnerFloat followedObjAngleSpinner;
    public Button camBtnPreviewAndAdjustFollowView;

    [HideInInspector]
    public bool isPreviewFollowedObj;
    int followedObjIndex;
    string followedObjPathGuid;
    [HideInInspector]
    public float followedObjDistance = 5f;
    [HideInInspector]
    public float followedObjHeight = 2f;
    [HideInInspector]
    public float followedObjAngle = 0f;

    [Header("Sounds")]
    public SoundManager soundManager;
    public AudioSource audioSource;
    public ListViewSound listViewSound;
    public Button btnAddNewSound;

    [Header("Walking people")]
    public GameObject[] peoplePrefabs;
    public ListViewWalkingPeople listViewWalkingPeople;
    public Button btnAddNewWalkingPeople;
    public WaypointCreator wpCreator;

    [Header("People with actions")]
    public GameObject spawnPlanePrefab;
    public GameObject[] specialPeoplePrefabs;
    public PickerIcons prefabsPicker;    
    public ListViewObjectWithAction listViewObjectWithAction;
    public Button btnAddNewObjectWithAction;    
    public PointOnPathDataManager popManager;
    int currentEditObjectIndex;
    int currentEditPointOnPathIndex;
    public List<HumanAIController> allAPScripts = new List<HumanAIController>();//stored all people with actions scripts

    [Header("General settings")]
    public FileDialog scenarioSelectFileDialog;
    public InputField scenarioName;
    public InputField scenarioFile;
    public Slider musicSoundSlider;
    public Slider talkingSoundSlider;
    public Slider streetSoundSlider;
    public Slider otherSoundSlider;
    public Button btnPreviewScenario;
    public Button btnPreviewScenarioOnlySelected;
    public Button btnSaveScenario;
    public Button btnBackToMain;
    bool isPreviewScenario;

    [Header("Play settings")]
    public Slider play_MusicSoundSlider;
    public Slider play_TalkingSoundSlider;
    public Slider play_StreetSoundSlider;
    public Slider play_OtherSoundSlider;

    void Start()
    {
        DevTest();

        AudioListener.volume = 1;

        SetupUI();
        GetScenarioData();
    }

    void DevTest()
    {
#if UNITY_EDITOR        
        if (FindObjectOfType<ResourceManager>() == null)
        {
            var go = new GameObject("ResourceManager");
            var rm = go.AddComponent<ResourceManager>();
            rm.file = "tinhhuong1";
            rm.mode = BtnScenario.ScenarioLoadType.Build;
        }
#endif
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) ShowHideMenu();
        ScaleMenu();
    }

    public void ShowHideMenu()
    {
        if (sceneMode == BtnScenario.ScenarioLoadType.Build && buildingMenu != null)
        {
            buildingMenu.SetActive(!buildingMenu.activeSelf);

            if (buildingMenu.activeSelf && popManager.gameObject.activeSelf)
            {
                listViewObjectWithAction.RefreshView(currentEditObjectIndex, currentEditPointOnPathIndex, true);
            }
        }
        else if (sceneMode == BtnScenario.ScenarioLoadType.Play && playingMenu != null)
            playingMenu.SetActive(!playingMenu.activeSelf);

        locationPopup.SetActive(false);
    }

    public void ScaleMenu()
    {
        if (sceneMode == BtnScenario.ScenarioLoadType.Build && buildingMenu != null)
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                buildingMenuScaleFactor = Mathf.Min(1f, buildingMenuScaleFactor + 0.1f);
                buildingMenu.transform.localScale = Vector3.one * buildingMenuScaleFactor;
            }
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                buildingMenuScaleFactor = Mathf.Max(0.2f, buildingMenuScaleFactor - 0.1f);
                buildingMenu.transform.localScale = Vector3.one * buildingMenuScaleFactor;
            }
            if (Input.GetKeyDown(KeyCode.KeypadMultiply))
            {
                buildingMenuScaleFactor = 1;
                buildingMenu.transform.localScale = Vector3.one * buildingMenuScaleFactor;
            }
        }
    }

    public void ShowHideLocationPopup()
    {
        if (sceneMode == BtnScenario.ScenarioLoadType.Build && locationPopup != null)
            locationPopup.SetActive(!locationPopup.activeSelf);
    }

    public void ShowHelpDocument()
    {
        Application.OpenURL((Application.dataPath) + Helper.Help_DataFolder + "\\help.chm");
    }

    //INIT
    void SetupUI()
    {
        if (resourceMgr == null) resourceMgr = FindObjectOfType<ResourceManager>();        
        if (mapButton != null) mapButton.SetActive(!hideMapButton && resourceMgr.mode == BtnScenario.ScenarioLoadType.Build);
        if (buildingMenu != null) buildingMenu.SetActive(false);
        if (playingMenu != null) playingMenu.SetActive(false);

        buildingMenuScaleFactor = 1;

        //Main camera
        if (mainCamera == null) mainCamera = Camera.main;

        //Setup select music file dialog
        if (musicSelectFileDialog != null)
        {
            musicSelectFileDialog.TemplateName = "music file select";
            musicSelectFileDialog.FileListView.FilePatterns = Helper.MusicFileSelectPatterns;
            musicSelectFileDialog.FileListView.CurrentDirectory = string.Format("{0}\\{1}", Application.dataPath, Helper.Sounds_DataFolder);
            musicSelectFileDialog.OkButton.onClick.AddListener(OnMusicFileSelected);
        }

        //Setup camera
        if (camBtnPreviewAndAdjustFollowView != null) camBtnPreviewAndAdjustFollowView.onClick.AddListener(PreviewAndAdjustFollowObjDirection);

        //Setup sound
        if (btnAddNewSound != null) btnAddNewSound.onClick.AddListener(AddNewSound);

        //Setup walking people
        if (btnAddNewWalkingPeople != null) btnAddNewWalkingPeople.onClick.AddListener(AddNewWalkingPeople);

        //Setup people with action
        if (prefabsPicker != null) prefabsPicker.TemplateName = "prefab picker";        
        if (btnAddNewObjectWithAction != null) btnAddNewObjectWithAction.onClick.AddListener(AddNewObjectWithAction);
        if (popManager != null) popManager.gameObject.SetActive(false);

        currentEditObjectIndex = -1;
        currentEditPointOnPathIndex = -1;

        //General setup
        //Setup select scenario file dialog
        if (scenarioSelectFileDialog != null)
        {
            //scenarioSelectFileDialog.TemplateName = "scenario file select";
            //scenarioSelectFileDialog.FileListView.FilePatterns = Helper.ScenarioFileSelectPatterns;
            //scenarioSelectFileDialog.FileListView.CurrentDirectory = string.Format("{0}\\{1}", Application.dataPath, Helper.Scenarios_DataFolder);
            //scenarioSelectFileDialog.OkButton.onClick.AddListener();
        }

        if (btnPreviewScenario != null) btnPreviewScenario.onClick.AddListener(() => PreviewScenerio(false));
        if (btnPreviewScenarioOnlySelected != null) btnPreviewScenarioOnlySelected.onClick.AddListener(() => PreviewScenerio(true));
        if (btnSaveScenario != null) btnSaveScenario.onClick.AddListener(SaveScenarioData);
        if (btnBackToMain != null) btnBackToMain.onClick.AddListener(BackToMainScene);
    }

    void GetScenarioData()
    {
        sceneMode = resourceMgr.mode;
        var scFileName = resourceMgr.file;
        var scFilePath = string.Format("{0}\\{1}\\{2}", Application.dataPath, Helper.Scenarios_DataFolder, scFileName + ".ini");        
        var scenario = new IniFile(scFilePath);

        //////////
        //GET GENERAL DATA        
        scenarioName.text = scenario.Read(Helper.Sc_Key_ScenarioName, Helper.Sc_Section_General);
        scenarioFile.text = scFileName;

        if (sceneMode == BtnScenario.ScenarioLoadType.Build)
        {
            musicSoundSlider.value = scenario.ReadFloat(Helper.Sc_Key_MusicSound, Helper.Sc_Section_General, 1);
            talkingSoundSlider.value = scenario.ReadFloat(Helper.Sc_Key_TalkingSound, Helper.Sc_Section_General, 1);
            streetSoundSlider.value = scenario.ReadFloat(Helper.Sc_Key_StreetSound, Helper.Sc_Section_General, 1);
            otherSoundSlider.value = scenario.ReadFloat(Helper.Sc_Key_OtherSound, Helper.Sc_Section_General, 1);
        }
        else
        {
            play_MusicSoundSlider.value = scenario.ReadFloat(Helper.Sc_Key_MusicSound, Helper.Sc_Section_General, 1);
            play_TalkingSoundSlider.value = scenario.ReadFloat(Helper.Sc_Key_TalkingSound, Helper.Sc_Section_General, 1);
            play_StreetSoundSlider.value = scenario.ReadFloat(Helper.Sc_Key_StreetSound, Helper.Sc_Section_General, 1);
            play_OtherSoundSlider.value = scenario.ReadFloat(Helper.Sc_Key_OtherSound, Helper.Sc_Section_General, 1);
        }
        

        //////////
        //GET SOUND DATA
        int soundCount = scenario.ReadInt(Helper.Sc_Key_SoundCount, Helper.Sc_Section_Sound, 0);
        var soundList = new ObservableList<ListViewSoundItemDescription>();
        for (int i = 0; i < soundCount; i++)
        {
            try
            {
                //Get
                var sectionName = string.Format("{0}{1}", Helper.Sc_Section_SoundDetailPrefix, i + 1);
                
                var soundInfo = scenario.Read(Helper.Sc_Key_SoundInfo, sectionName);
                int fileCount = scenario.ReadInt(Helper.Sc_Key_SoundFileCount, sectionName, 0);
                var fileList = new ObservableList<string>();
                for (int j = 0; j < fileCount; j++)
                {
                    try
                    {
                        var key = string.Format("{0}{1}", Helper.Sc_Key_SoundFileInfoPrefix, j + 1);
                        var file = scenario.Read(key, sectionName);
                        fileList.Add(file);
                    }
                    catch (System.Exception) { }
                }

                //Parse
                var arr = soundInfo.Split('|');
                var pos = arr[1].Replace("(", "").Replace(")", "").Split(',');

                var item = new ListViewSoundItemDescription
                {
                    SoundIndex = i,
                    SoundNote = arr[2],
                    HaveSoundPos = bool.Parse(arr[0]),
                    SoundPosition = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])),
                    SoundDistance = float.Parse(arr[3]),
                    SoundFiles = fileList,
                    SoundType = (SoundControlType)int.Parse(arr[4])
                };

                soundList.Add(item);
            }
            catch (System.Exception) { }
        }
        listViewSound.LoadDataFromList(soundList);

        //////////
        //GET WALKING PEOPLE DATA
        int wpCount = scenario.ReadInt(Helper.Sc_Key_WPCount, Helper.Sc_Section_WalkingPeople, 0);
        var wpList = new ObservableList<ListViewWalkingPeopleItemDescription>();
        for (int i = 0; i < wpCount; i++)
        {
            try
            {
                //Get
                var sectionName = string.Format("{0}{1}", Helper.Sc_Section_WalkingPeopleDetailPrefix, i + 1);

                var wpInfo = scenario.Read(Helper.Sc_Key_WPInfo, sectionName);
                int pointCount = scenario.ReadInt(Helper.Sc_Key_WPPointCount, sectionName, 0);
                var points = new ObservableList<Vector3>();
                for (int j = 0; j < pointCount; j++)
                {
                    try
                    {
                        var key = string.Format("{0}{1}", Helper.Sc_Key_WPPointInfoPrefix, j + 1);
                        var pStr = scenario.Read(key, sectionName);
                        var pArr = pStr.Replace("(", "").Replace(")", "").Split(',');

                        points.Add(new Vector3(float.Parse(pArr[0]), float.Parse(pArr[1]), float.Parse(pArr[2])));
                    }
                    catch (System.Exception) { }
                }

                //Parse
                var arr = wpInfo.Split('|');                

                var item = new ListViewWalkingPeopleItemDescription
                {
                    WalkingPeopleIndex = i,
                    Note = arr[0],                    
                    HavePath = !string.IsNullOrEmpty(arr[1]),
                    PathGUID = arr[1],
                    CreateAlongPath = bool.Parse(arr[2]),
                    IsLoop = bool.Parse(arr[3]),
                    IsReverse = bool.Parse(arr[4]),
                    Density = float.Parse(arr[5]),
                    TimeInterval = float.Parse(arr[6]),
                    Points = points,
                    IsShowed = false,
                    IsPreviewed = false
                };

                wpList.Add(item);

                //Create path in scene
                wpCreator.CreatePathFromPoints(points, item.PathGUID, false);
            }
            catch (System.Exception) { }
        }
        listViewWalkingPeople.LoadDataFromList(wpList);


        //////////
        //GET ACTING PEOPLE DATA
        int apCount = scenario.ReadInt(Helper.Sc_Key_APCount, Helper.Sc_Section_PeopleWithAction, 0);
        var apList = new ObservableList<ListViewObjectWithActionItemDescription>();
        ListViewObjectWithActionItemDescription followedObj = null;
        for (int i = 0; i < apCount; i++)
        {
            try
            {
                //Get
                var sectionName = string.Format("{0}{1}", Helper.Sc_Section_PeopleWithActionDetailPrefix, i + 1);

                var apInfo = scenario.Read(Helper.Sc_Key_APInfo, sectionName);                

                int pointCount = scenario.ReadInt(Helper.Sc_Key_APPointCount, sectionName, 0);
                var points = new ObservableList<Vector3>();
                var pointsData = new ObservableList<HWaypoint>();
                for (int j = 0; j < pointCount; j++)
                {
                    try
                    {
                        var key = string.Format("{0}{1}", Helper.Sc_Key_APPointInfoPrefix, j + 1);
                        var pInfo = scenario.Read(key, sectionName);

                        var pData = new HWaypoint(pInfo);
                        points.Add(pData.posV3);

                        //add talking clip to list
                        if (pData.haveSoundAtPos)
                            soundManager.AddTalkingClip(pData.soundAtPosPath);

                        pointsData.Add(pData);
                    }
                    catch (System.Exception) { }
                }

                //Parse people data                
                var item = new ListViewObjectWithActionItemDescription(apInfo);
                item.ObjectIndex = i;
                item.PointsData = pointsData;
                item.CalculateApproxTimeOnPath();

                if (item.IsFollowByCamera)
                    followedObj = item;

                apList.Add(item);

                //Create path in scene
                wpCreator.CreatePathFromPoints(points, item.PathGUID, false);
            }
            catch (System.Exception) { }
        }
        listViewObjectWithAction.LoadDataFromList(apList);

        //////////
        //GET CAMERA DATA
        followedObjDistance = scenario.ReadFloat(Helper.Sc_Key_CamDistance, Helper.Sc_Section_Camera, 5);
        followedObjHeight = scenario.ReadFloat(Helper.Sc_Key_CAmHeight, Helper.Sc_Section_Camera, 2);
        followedObjAngle = scenario.ReadFloat(Helper.Sc_Key_CamAngle, Helper.Sc_Section_Camera, 0);

        followedObjDistanceSpinner.Value = followedObjDistance;
        followedObjHeightSpinner.Value = followedObjHeight;
        followedObjAngleSpinner.Value = followedObjAngle;

        UpdateFollowedObjectInfo(followedObj);

        //PLAY IF NEEDED
        if (sceneMode == BtnScenario.ScenarioLoadType.Play)
            PreviewScenerio(false);
    }

    //Dialog
    [HideInInspector]
    public ConfirmDialogStatus confirmStatus;
    public void ShowConfirmDialog(string msg)
    {
        confirmStatus = ConfirmDialogStatus.Waiting;
        var actions = new DialogButton[]
            {
                new DialogButton("CÓ", ConfirmDialog_Yes),
                new DialogButton("KHÔNG", ConfirmDialog_No),
                //new DialogButton("Cancel", DialogBase.DefaultClose),
            };

        yesNoDialog.Clone().Show(
            title: msg,
            message: "",
            buttons: actions,
            focusButton: "KHÔNG",
            modal: true,
            modalColor: new Color(0, 0, 0, 0.8f),
            icon: null);
    }

    bool ConfirmDialog_Yes(DialogBase dialog, int buttonIndex)
    {
        confirmStatus = ConfirmDialogStatus.ConfirmYes;
        return true;
    }

    bool ConfirmDialog_No(DialogBase dialog, int buttonIndex)
    {
        confirmStatus = ConfirmDialogStatus.ConfirmNo;
        return true;
    }

    public void ShowInfoDialog(string msg)
    {
        var actions = new DialogButton[]
            {
                new DialogButton("OK", DialogBase.DefaultClose),                
                //new DialogButton("Cancel", DialogBase.DefaultClose),
            };

        infoDialog.Clone().Show(
            title: msg,
            message: "",
            buttons: actions,
            focusButton: "OK",
            modal: true,
            modalColor: new Color(0, 0, 0, 0.8f)
            //icon: null
            );
    }

    //MUSIC SELECT
    public void ShowMusicSelectFileDialog(int soundItemIndex)
    {
        selectMusicType = ShowMusicSelectDialogType.Common;

        //activate modal helper
        modalHelper.SetActive(true);
        modalHelper.transform.SetAsLastSibling();

        //activate music select dialog
        musicSelectFileDialog.gameObject.SetActive(true);
        musicSelectFileDialog.transform.SetAsLastSibling();//Bring to front

        currentSoundItemIndex = soundItemIndex;
    }

    public void ShowMusicSelectFileDialog_ForActionAtPos()
    {
        selectMusicType = ShowMusicSelectDialogType.ActionAtPos;

        //activate modal helper
        modalHelper.SetActive(true);
        modalHelper.transform.SetAsLastSibling();

        //activate music select dialog
        musicSelectFileDialog.gameObject.SetActive(true);
        musicSelectFileDialog.transform.SetAsLastSibling();//Bring to front
    }

    void OnMusicFileSelected()
    {
        musicSelectFileDialog.Cancel();

        //deactivate modal helper
        modalHelper.SetActive(false);

        var file = musicSelectFileDialog.FileListView.SelectedItem;
        if (file != null && file.IsFile)
        {
            selectedFileName = file.DisplayName;
            var pArr = file.FullName.Split(new[] { Helper.Sounds_DataFolder }, StringSplitOptions.None);
            if (pArr.Length == 2)
            {
                selectedFileFullPath = pArr[1];

                if (selectMusicType == ShowMusicSelectDialogType.Common)
                {
                    listViewSound.MusicFileSelectedCompleted(currentSoundItemIndex, selectedFileFullPath);
                }
                else if (selectMusicType == ShowMusicSelectDialogType.ActionAtPos)
                {
                    popManager.UpdateSoundAtPos(selectedFileFullPath);
                }   
            }
        }
        else
        {
            selectedFileName = string.Empty;
            selectedFileFullPath = string.Empty;
            currentSoundItemIndex = -1;
        }
        
    }

    public IEnumerator PlayMusicFile(string filePath)
    {
        if (audioSource.isPlaying) audioSource.Stop();

        yield return null;

        var clip = Helper.LoadClipFromFile(filePath);

        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    public void PausePlayingMusicFile()
    {
        audioSource.Stop();
    }

    //CAMERA
    void PreviewCamera()
    {
        if (!isPreviewScenario)
        {
            StartCoroutine(ChangeBackToFlyCam(0));
        }
        else
        {
            //get people to follow
            var followedObjGuid = listViewObjectWithAction.GetFollowedByCamObjGuid();
            if (!string.IsNullOrWhiteSpace(followedObjGuid))
            {
                var followedObjScripts = allAPScripts.FirstOrDefault(s => s.ObjGuid == followedObjGuid);

                if (followedObjScripts != null)
                    camControl.FollowObject(followedObjScripts.transform, followedObjDistance, followedObjHeight, followedObjAngle);
            }
            
        }
    }

    public void UpdateFollowedObjectInfo(ListViewObjectWithActionItemDescription objData)
    {
        bool haveFollowedObj = objData != null && objData.IsFollowByCamera;

        followedObjIndex = haveFollowedObj ? objData.ObjectIndex : -1;
        followedObjPathGuid = haveFollowedObj ? objData.PathGUID : string.Empty;
        string followedObjPrefabId = objData != null ? objData.ObjectPrefabId : string.Empty;
        string followedObjName = objData != null ? objData.ObjectName : string.Empty;

        camHaveFollowObjIcon.SetActive(haveFollowedObj);
        camNotHaveFollowObjIcon.SetActive(!haveFollowedObj);
        camBtnPreviewAndAdjustFollowView.interactable = haveFollowedObj;

        if (haveFollowedObj)
        {
            var icon = Resources.Load<Texture2D>(Helper.PrefabImages_ResourcesFolder + followedObjPrefabId);
            camFollowObjPrefabIcon.texture = icon;
            camFollowObjName.text = followedObjName;
        }
        else
        {
            camFollowObjPrefabIcon.texture = null;
            camFollowObjName.text = string.Empty;
        }
    }

    public void PreviewAndAdjustFollowObjDirection()
    {
        if (followedObjIndex == -1) return;

        isPreviewFollowedObj = !isPreviewFollowedObj;
        listViewObjectWithAction.PreviewObjectPath(followedObjIndex);

        camBtnPreviewAndAdjustFollowView.GetComponentInChildren<Text>().text = isPreviewFollowedObj ? "Dừng" : "Xem thử và điều chỉnh góc đi theo";

        if (isPreviewFollowedObj)
        {
            //get people to follow
            var followedObjGuid = listViewObjectWithAction.GetFollowedByCamObjGuid();
            if (!string.IsNullOrWhiteSpace(followedObjGuid))
            {
                var followedObjScripts = allAPScripts.FirstOrDefault(s => s != null && s.ObjGuid == followedObjGuid);
                if (followedObjScripts != null)
                    camControl.FollowObject(followedObjScripts.transform, followedObjDistance, followedObjHeight, followedObjAngle);
            }
        }
        else
        {
            StartCoroutine(ChangeBackToFlyCam(0));
        }
    }

    public void AdjustFollowObjDirection(float dis, float height, float angle)
    {
        followedObjDistance = dis;
        followedObjHeight = height;
        followedObjAngle = angle;

        followedObjDistanceSpinner.Value = dis;
        followedObjHeightSpinner.Value = height;
        followedObjAngleSpinner.Value = angle;
    }

    //SOUND
    void AddNewSound()
    {
        listViewSound.AddNewSound();
    }

    //PATH: CREATIONS && PREVIEW
    public void CreateNewPath(string oldGuid, string newGuid, CreatePathType type)
    {
        isCreatingPath = true;
        currentCreatePathType = type;
        wpCreator.CreatePathManually(oldGuid, newGuid);
    }

    public void FinishCreateNewPath(ObservableList<Vector3> points)
    {
        isCreatingPath = false;
        switch (currentCreatePathType)
        {
            case CreatePathType.WalkingPeople:
                listViewWalkingPeople.FinishCreateNewPath(points);
                break;
            case CreatePathType.ObjectWithAction:
                listViewObjectWithAction.FinishCreateObjectPath(points);
                break;
            default:
                break;
        }
    }

    public void CancelCreateNewPath()
    {
        isCreatingPath = false;
        switch (currentCreatePathType)
        {
            case CreatePathType.WalkingPeople:
                listViewWalkingPeople.CancelCreateNewPath();
                break;
            case CreatePathType.ObjectWithAction:
                listViewObjectWithAction.CancelCreateNewPath();
                break;            
            default:
                break;
        }
    }

    public void ShowHidePath(string guid, bool isShow)
    {
        wpCreator.ShowHidePath(guid, isShow);

        //hide direction line
        if (!isShow) popManager.HideDirLine();

        //refresh view
        listViewObjectWithAction.RefreshView(currentEditObjectIndex, currentEditPointOnPathIndex, popManager.gameObject.activeSelf);
    }

    public void ViewFirstPointOnPath(string guid)
    {
        var firstPoint = wpCreator.GetFirstPointOnPath(guid);
        if (firstPoint != null)
        {
            camControl.followCam.player = firstPoint;
            camControl.cameraType = CameraControlType.FollowPlayer;
            camControl.SetCameraType();

            StartCoroutine(ChangeBackToFlyCam(0));
        }
    }

    IEnumerator ChangeBackToFlyCam(float time)
    {
        yield return new WaitForSeconds(time);

        camControl.cameraType = CameraControlType.FlyCamera;
        camControl.SetCameraType();
    }
    
    public void DeletePathData(string guid)
    {
        //Delete people on path
        foreach (var p in wpCreator.paths)
        {
            if (p != null && p.GetComponent<ObjectIdentifier>().Id == guid)
            {
                HumanAISpawner hs = p.GetComponent<HumanAISpawner>();
                if (hs != null && hs.peopleContainer != null)
                    DestroyImmediate(hs.peopleContainer);

                break;
            }
        }

        //Delete path
        wpCreator.DeletePath(guid);
    }

    //WALKING PEOPLE
    void AddNewWalkingPeople()
    {
        listViewWalkingPeople.AddNewWalkingPeople();
    }
    
    public void PreviewPath(string guid, bool isPreviewd, bool createAlongPath, bool isLoop, bool isReverse, float density, float interval)
    {
        foreach (var p in wpCreator.paths)
        {
            if (p != null && p.GetComponent<ObjectIdentifier>().Id == guid)
            {
                HumanWaypoint hw = p.GetComponent<HumanWaypoint>();
                HumanAISpawner hs = p.GetComponent<HumanAISpawner>();
                if (hw == null) hw = p.AddComponent<HumanWaypoint>();
                if (hs == null) hs = p.AddComponent<HumanAISpawner>();
                

                if (hs.peopleContainer != null)
                    DestroyImmediate(hs.peopleContainer);

                if (isPreviewd)
                {
                    hs.aiPrefabs = peoplePrefabs;
                    hs.way = hw;

                    //Spawn people on path if have at least 2 points
                    if (p.GetComponent<PathManager>().waypoints.Length >= 2)
                        hs.SpawnWalkingPeopleOnPath(p.GetComponent<PathManager>().waypoints, createAlongPath, isLoop, isReverse, density, interval);
                }

                break;
            }
        }
    }

    //OBJECT WITH ACTION
    void AddNewObjectWithAction()
    {
        //activate modal helper
        modalHelper.SetActive(true);
        modalHelper.transform.SetAsLastSibling();

        //activate prefab picker
        prefabsPicker.gameObject.SetActive(true);
        prefabsPicker.transform.SetAsLastSibling();

        LayoutRebuilder.ForceRebuildLayoutImmediate(prefabsPicker.gameObject.GetComponent<RectTransform>());
        Vector2 ps = prefabsPicker.gameObject.GetComponent<RectTransform>().sizeDelta;

        //Set popup position
        prefabsPicker.gameObject.GetComponent<RectTransform>().position = scenarioMenuAnchor.position + new Vector3(ps.x / 2f, -ps.y / 2f, 0);
    }

    public void FinishPickPrefab()
    {
        //deactivate modal helper
        modalHelper.SetActive(false);

        //deactivate prefab picker
        prefabsPicker.gameObject.SetActive(false);

        if (prefabsPicker.ListView.SelectedItem.Icon != null)
        {
            var prefabId = prefabsPicker.ListView.SelectedItem.Icon.name;
            if (!string.IsNullOrEmpty(prefabId))
                listViewObjectWithAction.AddNewObjectWithAction(prefabId);
        }
    }

    public void EditPointOnPathData(int objIndex, string pathGuid, int totalPointOnPath, int pointIndex, HWaypoint pointData, string objName)
    {
        if (currentEditObjectIndex == objIndex 
            && currentEditPointOnPathIndex == pointIndex 
            && popManager.gameObject.activeSelf) //close current editing
        {
            if (popManager.HasChanged())
                StartCoroutine(EditPointOnPathData_ApplyCurrentChangedAndCloseAsync());
            else
                popManager.gameObject.SetActive(false);
        }
        else 
        {
            popManager.gameObject.SetActive(true);

            LayoutRebuilder.ForceRebuildLayoutImmediate(popManager.gameObject.GetComponent<RectTransform>());
            Vector2 ps = popManager.gameObject.GetComponent<RectTransform>().sizeDelta;

            //Set popup position
            popManager.gameObject.GetComponent<RectTransform>().position = scenarioMenuAnchor.position + new Vector3(ps.x / 2f, -ps.y / 2f, 0);

            //Pass data and load
            currentEditObjectIndex = objIndex;
            currentEditPointOnPathIndex = pointIndex;
            popManager.LoadData(pathGuid, totalPointOnPath, pointIndex, pointData, objName);
            
        }

        //refresh view
        listViewObjectWithAction.RefreshView(objIndex, pointIndex, popManager.gameObject.activeSelf);
    }

    IEnumerator EditPointOnPathData_ApplyCurrentChangedAndCloseAsync()
    {
        string msg = string.Format("Lưu thay đổi?");
        ShowConfirmDialog(msg);

        while (confirmStatus == ConfirmDialogStatus.Waiting)
        {
            yield return null;
        }

        if (confirmStatus == ConfirmDialogStatus.ConfirmYes)
        {
            popManager.BtnOKClicked();
        }
        else if (confirmStatus == ConfirmDialogStatus.ConfirmNo)
        {
            popManager.BtnCancelClicked();
        }
    }

    public void InsertNewPointOnPath(int newPointIndex)
    {
        currentEditPointOnPathIndex = newPointIndex;

        //refresh view
        listViewObjectWithAction.RefreshView(currentEditObjectIndex, currentEditPointOnPathIndex, true);
    }

    public Vector3 GetOtherPointPosOnPath(int otherPointIndex)
    {
        return listViewObjectWithAction.GetPointPos(currentEditObjectIndex, otherPointIndex);
    }

    public void FinishEditPointOnPathData(HWaypoint pointData)
    {
        popManager.gameObject.SetActive(false);

        if (pointData != null)
        {
            listViewObjectWithAction.FinishEditPointOnPathData(currentEditObjectIndex, currentEditPointOnPathIndex, pointData);

            //add talking clip to list
            if (pointData.haveSoundAtPos)
                soundManager.AddTalkingClip(pointData.soundAtPosPath);
        }

        //refresh view
        listViewObjectWithAction.RefreshView(currentEditObjectIndex, currentEditPointOnPathIndex, false);

        currentEditPointOnPathIndex = -1;
    }

    public void PreviewObjectOnPath(ListViewObjectWithActionItemDescription data)
    {
        foreach (var p in wpCreator.paths)
        {
            if (p != null && p.GetComponent<ObjectIdentifier>().Id == data.PathGUID)
            {
                HumanWaypoint hw = p.GetComponent<HumanWaypoint>();
                HumanAISpawner hs = p.GetComponent<HumanAISpawner>();
                if (hw == null) hw = p.AddComponent<HumanWaypoint>();
                if (hs == null) hs = p.AddComponent<HumanAISpawner>();
                hs.spawnPlane = spawnPlanePrefab;

                if (hs.peopleContainer != null)
                    DestroyImmediate(hs.peopleContainer);

                if (data.IsPreviewed)
                {
                    hs.aiPrefabs = peoplePrefabs.Union(specialPeoplePrefabs).ToArray();
                    hs.way = hw;

                    //Spawn ai
                    var aiScript = hs.SpawnPeopleWithActionOnPath(p.GetComponent<PathManager>().waypoints, data);
                    if (aiScript != null)
                        allAPScripts.Add(aiScript);
                }

                break;
            }
        }

        //refresh view
        listViewObjectWithAction.RefreshView(currentEditObjectIndex, currentEditPointOnPathIndex, popManager.gameObject.activeSelf);
    }

    //SCENARIO
    public void PreviewScenerio(bool selectedOnly)
    {
        isPreviewScenario = !isPreviewScenario;

        if (!selectedOnly)
        {
            btnPreviewScenario.GetComponentInChildren<Text>().text = isPreviewScenario ? "Dừng" : "Chạy thử toàn bộ";
            btnPreviewScenarioOnlySelected.interactable = !isPreviewScenario;
        }
        else
        {
            btnPreviewScenario.interactable = !isPreviewScenario;
            btnPreviewScenarioOnlySelected.GetComponentInChildren<Text>().text = isPreviewScenario ? "Dừng" : "Chạy thử theo lựa chọn";
        }
        btnSaveScenario.interactable = !isPreviewScenario;
        

        if (sceneMode == BtnScenario.ScenarioLoadType.Build && buildingMenu != null && isPreviewScenario)
            buildingMenu.SetActive(false);

        //Preview sound
        var soundContainer = GameObject.Find("Sound container");
        if (soundContainer != null) DestroyImmediate(soundContainer);
        if (isPreviewScenario)
        {
            soundContainer = new GameObject("Sound container");

            for (int i = 0; i < listViewSound.data.Count; i++)
            {
                var sound = new GameObject(listViewSound.data[i].SoundNote);
                sound.transform.parent = soundContainer.transform;
                sound.transform.position = listViewSound.data[i].SoundPosition;

                var sc = sound.AddComponent<SoundControl>();
                sc.soundType = listViewSound.data[i].SoundType;
                sc.playType = AudioPlayType.All_Shuffle_Forever;
                sc.soundMaxDistance = listViewSound.data[i].SoundDistance;

                var clips = new List<AudioClip>();
                for (int j = 0; j < listViewSound.data[i].SoundFiles.Count; j++)
                {
                    if (resourceMgr != null)
                    {
                        var fPath = string.Format("{0}{1}{2}", Application.dataPath, Helper.Sounds_DataFolder, listViewSound.data[i].SoundFiles[j]);
                        var clip = resourceMgr.GetClipByPath(fPath);
                        if (clip != null) clips.Add(clip);
                    }
                    else
                    {
                        Debug.Log("Read sound file manually");
                        var fPath = string.Format("{0}{1}{2}", Application.dataPath, Helper.Sounds_DataFolder, listViewSound.data[i].SoundFiles[j]);
                        var clip = Helper.LoadClipFromFile(fPath);
                        if (clip != null) clips.Add(clip);
                    }
                }
                sc.clips = clips.ToArray();

                sc.playOnAwake = true;
            }

            if (sceneMode == BtnScenario.ScenarioLoadType.Build)
            {
                soundManager.ApplySoundVolume(SoundControlType.Music, musicSoundSlider.value);
                soundManager.ApplySoundVolume(SoundControlType.Talking, talkingSoundSlider.value);
                soundManager.ApplySoundVolume(SoundControlType.Street, streetSoundSlider.value);
                soundManager.ApplySoundVolume(SoundControlType.Other, otherSoundSlider.value);
            }
            else
            {
                soundManager.ApplySoundVolume(SoundControlType.Music, play_MusicSoundSlider.value);
                soundManager.ApplySoundVolume(SoundControlType.Talking, play_TalkingSoundSlider.value);
                soundManager.ApplySoundVolume(SoundControlType.Street, play_StreetSoundSlider.value);
                soundManager.ApplySoundVolume(SoundControlType.Other, play_OtherSoundSlider.value);
            }
            
        }

        //Preview people
        //walking people
        for (int i = 0; i < listViewWalkingPeople.data.Count; i++) listViewWalkingPeople.PreviewPath(i, isPreviewScenario);

        //action people
        allAPScripts = new List<HumanAIController>();
        for (int i = 0; i < listViewObjectWithAction.data.Count; i++)
        {
            if (selectedOnly && !listViewObjectWithAction.data[i].IsIncludeInPreview) continue;

            listViewObjectWithAction.PreviewObjectPath(i, isPreviewScenario);
        }

        //preview camera
        PreviewCamera();
    }

    public void MusicSoundSliderChange(float value)
    {
        if (soundManager != null) soundManager.ApplySoundVolume(SoundControlType.Music, value);
    }

    public void TalkingSoundSliderChange(float value)
    {
        if (soundManager != null) soundManager.ApplySoundVolume(SoundControlType.Talking, value);
    }

    public void StreetSoundSliderChange(float value)
    {
        if (soundManager != null) soundManager.ApplySoundVolume(SoundControlType.Street, value);
    }

    public void OtherSoundSliderChange(float value)
    {
        if (soundManager != null) soundManager.ApplySoundVolume(SoundControlType.Other, value);
    }

    public void SaveScenarioData()
    {   
        var scFileName = scenarioFile.text + ".ini";
        var scFilePath = string.Format("{0}\\{1}\\{2}", Application.dataPath, Helper.Scenarios_DataFolder, scFileName);

        //Check file name
        var checkMsg = string.Empty;
        if (string.IsNullOrEmpty(scenarioFile.text)) checkMsg = "Chưa điền tên file tình huống!";
        if (scenarioFile.text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) checkMsg = "Tên file tình huống chứa ký tự đặc biệt!";

        if (!string.IsNullOrEmpty(checkMsg))
        {
            ShowInfoDialog(checkMsg);
            return;
        }

        //Save
        if (File.Exists(scFilePath))
            StartCoroutine(SaveScenarioDataAsync(scFilePath, scenarioFile.text));
        else
            SaveScenarioDataProcessing(scFilePath);
    }

    IEnumerator SaveScenarioDataAsync(string scFilePath, string fileName)
    {
        string msg = string.Format("File tình huống \"{0}\" đã tồn tại. Bạn có muốn lưu đè không?", fileName);
        ShowConfirmDialog(msg);

        while (confirmStatus == ConfirmDialogStatus.Waiting)
        {
            yield return null;
        }

        if (confirmStatus == ConfirmDialogStatus.ConfirmYes)
            SaveScenarioDataProcessing(scFilePath);
    }

    void SaveScenarioDataProcessing(string scFilePath)
    {
        var scenario = new IniFile(scFilePath);

        //////////
        //SAVE GENERAL DATA
        var scName = scenarioName.text;
        scenario.Write(Helper.Sc_Key_ScenarioName, scName, Helper.Sc_Section_General);

        scenario.Write(Helper.Sc_Key_MusicSound, musicSoundSlider.value.ToString(), Helper.Sc_Section_General);
        scenario.Write(Helper.Sc_Key_TalkingSound, talkingSoundSlider.value.ToString(), Helper.Sc_Section_General);
        scenario.Write(Helper.Sc_Key_StreetSound, streetSoundSlider.value.ToString(), Helper.Sc_Section_General);
        scenario.Write(Helper.Sc_Key_OtherSound, otherSoundSlider.value.ToString(), Helper.Sc_Section_General);

        //////////
        //SAVE CAMERA INFO
        scenario.Write(Helper.Sc_Key_CamDistance, followedObjDistance.ToString(), Helper.Sc_Section_Camera);
        scenario.Write(Helper.Sc_Key_CAmHeight, followedObjHeight.ToString(), Helper.Sc_Section_Camera);
        scenario.Write(Helper.Sc_Key_CamAngle, followedObjAngle.ToString(), Helper.Sc_Section_Camera);

        //////////
        //SAVE SOUND DATA
        //Delete old data
        var oldCount = scenario.ReadInt(Helper.Sc_Key_SoundCount, Helper.Sc_Section_Sound, 0);
        for (int i = 0; i < oldCount; i++)
        {
            var sectionName = string.Format("{0}{1}", Helper.Sc_Section_SoundDetailPrefix, i + 1);
            scenario.DeleteSection(sectionName);
        }
        //Write new data
        scenario.Write(Helper.Sc_Key_SoundCount, listViewSound.data.Count.ToString(), Helper.Sc_Section_Sound);
        for (int i = 0; i < listViewSound.data.Count; i++)
        {
            var sectionName = string.Format("{0}{1}", Helper.Sc_Section_SoundDetailPrefix, i + 1);

            var havePos = listViewSound.data[i].HaveSoundPos.ToString();
            var note = listViewSound.data[i].SoundNote;
            var distance = listViewSound.data[i].SoundDistance.ToString();
            var pos = listViewSound.data[i].SoundPosition.ToString();
            int soundType = (int)listViewSound.data[i].SoundType;
            var value = string.Format("{0}|{1}|{2}|{3}|{4}", havePos, pos, note, distance, soundType);
            scenario.Write(Helper.Sc_Key_SoundInfo, value, sectionName);

            scenario.Write(Helper.Sc_Key_SoundFileCount, listViewSound.data[i].SoundFiles.Count.ToString(), sectionName);
            for (int j = 0; j < listViewSound.data[i].SoundFiles.Count; j++)
            {
                var key = string.Format("{0}{1}", Helper.Sc_Key_SoundFileInfoPrefix, j + 1);
                scenario.Write(key, listViewSound.data[i].SoundFiles[j], sectionName);
            }
        }


        //////////
        //SAVE WALKING PEOPLE DATA
        //Delete old data
        oldCount = scenario.ReadInt(Helper.Sc_Key_WPCount, Helper.Sc_Section_WalkingPeople, 0);
        for (int i = 0; i < oldCount; i++)
        {
            var sectionName = string.Format("{0}{1}", Helper.Sc_Section_WalkingPeopleDetailPrefix, i + 1);
            scenario.DeleteSection(sectionName);
        }
        //Write new data
        scenario.Write(Helper.Sc_Key_WPCount, listViewWalkingPeople.data.Count.ToString(), Helper.Sc_Section_WalkingPeople);
        for (int i = 0; i < listViewWalkingPeople.data.Count; i++)
        {
            var sectionName = string.Format("{0}{1}", Helper.Sc_Section_WalkingPeopleDetailPrefix, i + 1);

            var note = listViewWalkingPeople.data[i].Note;
            var pathGuid = listViewWalkingPeople.data[i].PathGUID;
            var createAlong = listViewWalkingPeople.data[i].CreateAlongPath;
            var loop = listViewWalkingPeople.data[i].IsLoop.ToString();
            var reverse = listViewWalkingPeople.data[i].IsReverse;
            var density = listViewWalkingPeople.data[i].Density.ToString();
            var interval = listViewWalkingPeople.data[i].TimeInterval.ToString();

            var value = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}", note, pathGuid, createAlong, loop, reverse, density, interval);
            scenario.Write(Helper.Sc_Key_WPInfo, value, sectionName);
            scenario.Write(Helper.Sc_Key_WPPointCount, listViewWalkingPeople.data[i].Points.Count.ToString(), sectionName);
            for (int j = 0; j < listViewWalkingPeople.data[i].Points.Count; j++)
            {
                var key = string.Format("{0}{1}", Helper.Sc_Key_WPPointInfoPrefix, j + 1);
                value = string.Format("({0},{1},{2})", listViewWalkingPeople.data[i].Points[j].x, listViewWalkingPeople.data[i].Points[j].y, listViewWalkingPeople.data[i].Points[j].z);
                scenario.Write(key, value, sectionName);
            }
        }


        //////////
        //SAVE ACTING PEOPLE DATA
        //Delete old data
        oldCount = scenario.ReadInt(Helper.Sc_Key_APCount, Helper.Sc_Section_PeopleWithAction, 0);
        for (int i = 0; i < oldCount; i++)
        {
            var sectionName = string.Format("{0}{1}", Helper.Sc_Section_PeopleWithActionDetailPrefix, i + 1);
            scenario.DeleteSection(sectionName);
        }
        //Write new data
        scenario.Write(Helper.Sc_Key_APCount, listViewObjectWithAction.data.Count.ToString(), Helper.Sc_Section_PeopleWithAction);
        for (int i = 0; i < listViewObjectWithAction.data.Count; i++)
        {
            var sectionName = string.Format("{0}{1}", Helper.Sc_Section_PeopleWithActionDetailPrefix, i + 1);

            //people info
            scenario.Write(Helper.Sc_Key_APInfo, listViewObjectWithAction.data[i].GetSavedDataString(), sectionName);

            //point count
            scenario.Write(Helper.Sc_Key_APPointCount, listViewObjectWithAction.data[i].PointsData.Count.ToString(), sectionName);

            //each point info
            for (int j = 0; j < listViewObjectWithAction.data[i].PointsData.Count; j++)
            {
                var key = string.Format("{0}{1}", Helper.Sc_Key_APPointInfoPrefix, j + 1);
                scenario.Write(key, listViewObjectWithAction.data[i].PointsData[j].GetSavedDataString(), sectionName);
            }
        }


        ShowInfoDialog("Lưu tình huống thành công");
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
