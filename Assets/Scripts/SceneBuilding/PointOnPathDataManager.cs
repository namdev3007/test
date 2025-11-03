using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

public class PointOnPathDataManager : MonoBehaviour
{
    public Text title;

    [Space(3)]
    public Spinner otherPointIndexSpinner;
    public Text approxReachPointTime;

    [Space(3)]
    public ComboboxData actionAtPos;
    public InputField timeWaitAtPos;

    [Space(3)]
    public Button btnGetLookDir;
    public Button btnDirLeft;
    public Button btnDirRight;
    public Image haveLookDirIcon;
    public Image notHaveLookDirIcon;

    [Space(3)]
    public Button btnDeleteSound;
    public Button btnPlaySound;
    public Button btnPauseSound;
    public Image haveSoundIcon;
    public Image notHaveSoundIcon;
    public Text soundAtPosLength;
    public Text soundAtPosName;

    [Space(10)]
    public ComboboxData actionWhileGoToNextPos;
    public InputField speedGoToNextPos;

    [Space(10)]
    public SceneBuildingManager sbManager;
    public WaypointCreator wCreator;

    public HWaypoint currentPointData;    
    string pathGuid;
    int totalPointOnPath;
    string objName;
    int pointIndex;
    Vector3 originalPosV3;
    Vector3 posV3;
    bool haveLookDir;
    float lookAngle;
    bool haveSound;
    public Material dirLineMaterial;
    LineRenderer dirLine;

    void Update()
    {
        ChangeLookAngle();
        ChangePointPosition();

        if (Input.GetKeyDown(KeyCode.LeftShift)) holdBtnShift = true;
        if (Input.GetKeyUp(KeyCode.LeftShift)) holdBtnShift = false;
    }

    public void OnTimeWaitChanged(string value)
    {
        if (!float.TryParse(value, out float time))
            timeWaitAtPos.text = string.IsNullOrEmpty(value) ? "0" : value.Substring(0, value.Length - 1);
    }

    public void OnSpeedChanged(string value)
    {
        if (!float.TryParse(value, out float time))
            speedGoToNextPos.text = string.IsNullOrEmpty(value) ? "0" : value.Substring(0, value.Length - 1);
    }

    public void LoadData(string pathGuid, int totalPointOnPath, int editPointIndex, HWaypoint pointData, string objName)
    {
        this.currentPointData = pointData;        
        this.pathGuid = pathGuid;
        this.totalPointOnPath = totalPointOnPath;
        this.objName = objName;
        this.pointIndex = editPointIndex;

        title.text = string.Format("Thiết lập tham số <color=#ffff00><b>{0} - Điểm {1}</b></color>", objName, editPointIndex + 1);
        approxReachPointTime.text = pointData.approxTimeReachPoint.ToString("0.00");
        otherPointIndexSpinner.Max = totalPointOnPath;

        actionAtPos.PopulateData();

        actionAtPos.SelectItem(pointData.animAtPos);
        timeWaitAtPos.text = pointData.timeWaitAtPos.ToString();

        btnGetLookDir.GetComponentInChildren<Text>().text = pointData.haveLookDir ? "Xóa" : "Lấy";
        btnDirLeft.interactable = pointData.haveLookDir;
        btnDirRight.interactable = pointData.haveLookDir;
        haveLookDirIcon.gameObject.SetActive(pointData.haveLookDir);
        notHaveLookDirIcon.gameObject.SetActive(!pointData.haveLookDir);

        actionWhileGoToNextPos.SelectItem(pointData.animWhileGoingToNextPos);
        speedGoToNextPos.text = pointData.speedToNextPos.ToString();

        posV3 = pointData.posV3;
        originalPosV3 = pointData.posV3;
        haveLookDir = pointData.haveLookDir;
        lookAngle = pointData.lookAngle;
        UpdateSoundAtPos(pointData.soundAtPosPath);

        DrawDirLine();
    }

    public bool HasChanged()
    {
        if (currentPointData == null) return false;

        var pointData = new HWaypoint
        {
            posV3 = posV3,
            animAtPos = actionAtPos.GetSelectedValue(),
            timeWaitAtPos = float.Parse(timeWaitAtPos.text),
            animWhileGoingToNextPos = actionWhileGoToNextPos.GetSelectedValue(),
            speedToNextPos = float.Parse(speedGoToNextPos.text),
            haveLookDir = haveLookDir,
            lookAngle = lookAngle,
            haveSoundAtPos = haveSound,
            soundAtPosPath = soundAtPosName.text.Trim()
        };

        return !pointData.Equals(currentPointData);
    }

    void DrawDirLine()
    {
        if (dirLine == null)
        {
            var go = new GameObject("DirLine");
            dirLine = go.AddComponent<LineRenderer>();
        }

        if (haveLookDir)
        {
            Vector3 start = posV3;
            Vector3 nor = start.normalized;
            Vector3 end = start + Quaternion.Euler(0, lookAngle, 0) * nor;

            Vector3[] points = new Vector3[] { start, end };
            dirLine.positionCount = 2;
            dirLine.SetPositions(points);

            dirLine.material = dirLineMaterial;
            dirLine.startWidth = 0.05f;
            dirLine.endWidth = 0.05f;
        }
        else
            DestroyImmediate(dirLine.gameObject);
    }

    public void BtnGetDirClicked()
    {
        haveLookDir = !haveLookDir;

        btnGetLookDir.GetComponentInChildren<Text>().text = haveLookDir ? "Xóa" : "Lấy";
        btnDirLeft.interactable = haveLookDir;
        btnDirRight.interactable = haveLookDir;
        haveLookDirIcon.gameObject.SetActive(haveLookDir);
        notHaveLookDirIcon.gameObject.SetActive(!haveLookDir);

        if (!haveLookDir) lookAngle = 0;

        DrawDirLine();
    }

    //Change look angle
    bool holdBtnChangeAngle;
    bool changeAngleToLeft;
    public void BtnChangeLookAngleHold(bool toLeft)
    {
        holdBtnChangeAngle = true;
        changeAngleToLeft = toLeft;
    }
    public void BtnChangeLookAngleRelease()
    {
        holdBtnChangeAngle = false;
    }

    void ChangeLookAngle()
    {
        if (holdBtnChangeAngle)
        {
            lookAngle = changeAngleToLeft ? lookAngle - 1 : lookAngle + 1;

            DrawDirLine();
        }
    }

    //Change point position
    bool holdBtnChangePos;
    bool holdBtnShift;
    int isChangingPointPos;
    public void BtnChangePointPositionHold(int dir)
    {
        holdBtnChangePos = true;
        isChangingPointPos = dir;
    }
    public void BtnChangePointPositionRelease()
    {
        holdBtnChangePos = false;
    }

    void ChangePointPosition()
    {
        if (holdBtnChangePos)
        {
            var cf = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
            var cr = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z);

            float factor = holdBtnShift ? 5 : 1;

            switch (isChangingPointPos)
            {
                case 1://left
                    posV3 += cr * -0.01f * factor;
                    break;
                case 2://right
                    posV3 += cr * 0.01f * factor;
                    break;
                case 3://forward
                    posV3 += cf * 0.01f * factor;
                    break;
                case 4://back
                    posV3 += cf * -0.01f * factor;
                    break;
                case 5://up
                    posV3 += Vector3.up * 0.01f * factor;
                    break;
                case 6://down
                    posV3 += Vector3.down * 0.01f * factor;
                    break;
                default:
                    posV3 += Vector3.zero;
                    break;
            }

            wCreator.ChangePointPosition(pathGuid, pointIndex, posV3);

            DrawDirLine();
        }
    }

    public void UpdateSoundAtPos(string soundPath)
    {
        //display sound name
        haveSound = !string.IsNullOrWhiteSpace(soundPath);
        soundAtPosName.text = string.IsNullOrWhiteSpace(soundPath) ? "" : soundPath.Trim();

        //have sound icon
        haveSoundIcon.gameObject.SetActive(haveSound);
        notHaveSoundIcon.gameObject.SetActive(!haveSound);

        //get sound length
        var filePath = string.Format("{0}\\{1}\\{2}", Application.dataPath, Helper.Sounds_DataFolder, soundPath);
        var clip = Helper.LoadClipFromFile(filePath);
        soundAtPosLength.text = clip != null ? string.Format("{0}s", clip.length.ToString("0.00")) : "";

        //buttons
        btnDeleteSound.interactable = haveSound;
        btnPlaySound.interactable = haveSound;
        btnPauseSound.interactable = haveSound;

        btnPlaySound.gameObject.SetActive(true);
        btnPauseSound.gameObject.SetActive(false);
    }

    //Buttons handle

    public void BtnCancelClicked()
    {
        HideDirLine();        

        wCreator.ChangePointPosition(pathGuid, pointIndex, originalPosV3);
        sbManager.FinishEditPointOnPathData(null);

        currentPointData = null;
    }

    public void HideDirLine()
    {
        if (dirLine != null) DestroyImmediate(dirLine.gameObject);
    }

    public void BtnOKClicked()
    {
        if (dirLine != null) DestroyImmediate(dirLine.gameObject);

        HWaypoint pointData = new HWaypoint
        {
            posV3 = posV3,
            animAtPos = actionAtPos.GetSelectedValue(),
            timeWaitAtPos = float.Parse(timeWaitAtPos.text),
            animWhileGoingToNextPos = actionWhileGoToNextPos.GetSelectedValue(),
            speedToNextPos = float.Parse(speedGoToNextPos.text),
            haveLookDir = haveLookDir,
            lookAngle = lookAngle,
            haveSoundAtPos = haveSound,
            soundAtPosPath = soundAtPosName.text
        };

        sbManager.FinishEditPointOnPathData(pointData);

        currentPointData = null;
    }

    public void BtnInsertBeforeClicked()
    {
        if (sbManager.isCreatingPath) return;

        StartCoroutine(BtnInsertClickedAsync(true));
    }

    public void BtnInsertAfterClicked()
    {
        if (sbManager.isCreatingPath) return;

        StartCoroutine(BtnInsertClickedAsync(false));
    }

    public void BtnDeleteClicked()
    {
        if (sbManager.isCreatingPath) return;

        StartCoroutine(BtnDeleteClickedAsync());
    }

    IEnumerator BtnInsertClickedAsync(bool isBefore)
    {
        string msg = string.Format("Chèn thêm 1 điểm vào ngay {0} điểm số \"{1}\"?", isBefore ? "TRƯỚC" : "SAU", pointIndex + 1);
        sbManager.ShowConfirmDialog(msg);

        while (sbManager.confirmStatus == ConfirmDialogStatus.Waiting)
        {
            yield return null;
        }

        if (sbManager.confirmStatus == ConfirmDialogStatus.ConfirmYes)
        {
            sbManager.listViewObjectWithAction.InsertPointOnPath(pathGuid, pointIndex, isBefore);

            //adjust current point index if add new one before
            if (isBefore)
            {
                pointIndex += 1;
                title.text = string.Format("Thiết lập tham số <color=#ffff00><b>{0} - Điểm {1}</b></color>", objName, pointIndex + 1);
            }

            //refresh view
            sbManager.InsertNewPointOnPath(pointIndex);

            totalPointOnPath++;
            otherPointIndexSpinner.Max = totalPointOnPath;
        }
    }

    IEnumerator BtnDeleteClickedAsync()
    {
        string msg = string.Format("Xóa điểm số \"{0}\"?", pointIndex + 1);
        sbManager.ShowConfirmDialog(msg);

        while (sbManager.confirmStatus == ConfirmDialogStatus.Waiting)
        {
            yield return null;
        }

        if (sbManager.confirmStatus == ConfirmDialogStatus.ConfirmYes)
        {
            sbManager.listViewObjectWithAction.DeletePointOnPath(pathGuid, pointIndex);

            totalPointOnPath--;
            otherPointIndexSpinner.Max = totalPointOnPath;
        }
    }

    public void BtnDeleteSoundClicked()
    {
        if (sbManager.isCreatingPath) return;

        StartCoroutine(BtnDeleteSoundClickedAsync());
    }

    IEnumerator BtnDeleteSoundClickedAsync()
    {
        string msg = string.Format("Xóa âm thanh tại điểm số \"{0}\"?", pointIndex + 1);
        sbManager.ShowConfirmDialog(msg);

        while (sbManager.confirmStatus == ConfirmDialogStatus.Waiting)
        {
            yield return null;
        }

        if (sbManager.confirmStatus == ConfirmDialogStatus.ConfirmYes)
        {
            UpdateSoundAtPos(string.Empty);
        }
    }

    public void BtnPlaySoundClicked()
    {
        var filePath = string.Format("{0}\\{1}\\{2}", Application.dataPath, Helper.Sounds_DataFolder, soundAtPosName.text);

        StartCoroutine(sbManager.PlayMusicFile(filePath));
    }

    public void BtnPauseSoundClicked()
    {
        sbManager.PausePlayingMusicFile();
    }

    public void BtnTakeOtherPointPosClicked()
    {
        int otherPointIndex = otherPointIndexSpinner.Value - 1;
        var otherPointPos = sbManager.GetOtherPointPosOnPath(otherPointIndex);

        posV3 = otherPointPos;
        wCreator.ChangePointPosition(pathGuid, pointIndex, posV3);

        DrawDirLine();
    }
}
