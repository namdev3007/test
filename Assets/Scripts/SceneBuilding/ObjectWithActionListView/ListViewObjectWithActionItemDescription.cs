using System;
using UIWidgets;
using UnityEngine;

[Serializable]
public class ListViewObjectWithActionItemDescription
{
    [SerializeField]
    public string ObjectGUID;

    [SerializeField]
    public int ObjectIndex;

    [SerializeField]
    public string ObjectName;

    [SerializeField]
    public string ObjectPrefabId;

    [SerializeField]
    public Vector3 ObjectStartPos;

    [SerializeField]
    public bool HavePath;

    [SerializeField]
    public string PathGUID;

    [SerializeField]
    public bool IsShowPath;

    [SerializeField]
    public ObservableList<HWaypoint> PointsData;

    [SerializeField]
    public bool IsLoop;

    [SerializeField]
    public bool IsPreviewed;

    [SerializeField]
    public bool IsIncludeInPreview;

    [SerializeField]
    public bool IsFollowByCamera;

    public ListViewObjectWithActionItemDescription()
    {

    }

    public ListViewObjectWithActionItemDescription(string savedDataString)
    {
        var arr = savedDataString.Split('|');

        ObjectGUID = arr[0];
        ObjectName = arr[1];
        ObjectPrefabId = arr[2];
        PathGUID = arr[3];
        HavePath = !string.IsNullOrEmpty(PathGUID);
        IsFollowByCamera = bool.Parse(arr[4]);
    }

    public string GetSavedDataString()
    {
        var str = string.Format("{0}|{1}|{2}|{3}|{4}", ObjectGUID, ObjectName, ObjectPrefabId, PathGUID, IsFollowByCamera);
        return str;
    }

    public void CalculateApproxTimeOnPath()
    {
        if (PointsData == null || PointsData.Count == 0) return;

        PointsData[0].approxTimeReachPoint = 0;

        for (int i = 1; i < PointsData.Count; i++)
        {
            var prevPoint = PointsData[i - 1];
            var currentPoint = PointsData[i];

            float prevPointReachTime = prevPoint.approxTimeReachPoint;
            float prevPointWaitTime = prevPoint.timeWaitAtPos;

            float speedToCurrentPoint = prevPoint.speedToNextPos == 0 ? Helper.GetMovingSpeedByAnim(prevPoint.animWhileGoingToNextPos) : prevPoint.speedToNextPos;
            if (speedToCurrentPoint == 0) speedToCurrentPoint = 1;
            float disToCurrentPoint = Helper.DistanceXZ(prevPoint.posV3, currentPoint.posV3);
            float timeGoToCurrentPoint = disToCurrentPoint / speedToCurrentPoint;

            currentPoint.approxTimeReachPoint = prevPointReachTime + prevPointWaitTime + timeGoToCurrentPoint;
        }
    }
}
