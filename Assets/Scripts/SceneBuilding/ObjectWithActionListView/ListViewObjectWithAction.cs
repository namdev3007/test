using System;
using System.Collections;
using UIWidgets;
using UnityEngine;
using System.Linq;


public class ListViewObjectWithAction : ListViewCustom<ListViewObjectWithActionComponent, ListViewObjectWithActionItemDescription>
{
    readonly Comparison<ListViewObjectWithActionItemDescription> itemsComparison = (x, y) => UtilitiesCompare.Compare(x.ObjectIndex, y.ObjectIndex);

    bool isListViewObjectWithActionInited;

    public ObservableList<ListViewObjectWithActionItemDescription> data = new ObservableList<ListViewObjectWithActionItemDescription>();

    public SceneBuildingManager sceneBuildManager;
    public ListViewPointOnPath listViewPoints;
    public string currentCreatePathGUID;
    public int currentCreatePathIndex;

    /// <summary>
    /// Set items comparison.
    /// </summary>
    public override void Init()
    {
        if (isListViewObjectWithActionInited)
        {
            return;
        }

        isListViewObjectWithActionInited = true;

        base.Init();
        DataSource.Comparison = itemsComparison;
    }

    public void LoadDataFromList(ObservableList<ListViewObjectWithActionItemDescription> list)
    {
        data = list;

        this.DataSource = data;
    }

    public void AddNewObjectWithAction(string prefabId)
    {
        for (int i = 0; i < data.Count; i++)
        {
            data[i].ObjectIndex = i;
        }

        data.Add(new ListViewObjectWithActionItemDescription()
        {
            ObjectGUID = Guid.NewGuid().ToString().Replace("-", "_"),
            ObjectIndex = data.Count,
            ObjectPrefabId = prefabId
        });

        this.DataSource = data;
    }

    public void DeleteObjectWithActionRecord(int index)
    {
        StartCoroutine(DeleteObjectWithActionRecordAsync(index));
    }

    IEnumerator DeleteObjectWithActionRecordAsync(int index)
    {
        var item = data.Find(i => i.ObjectIndex == index);
        if (item != null)
        {
            string msg = string.Format("Bạn có muốn xóa đối tượng \"{0}\" không?", item.ObjectName);
            sceneBuildManager.ShowConfirmDialog(msg);

            while (sceneBuildManager.confirmStatus == ConfirmDialogStatus.Waiting)
            {
                yield return null;
            }

            if (sceneBuildManager.confirmStatus == ConfirmDialogStatus.ConfirmYes)
            {
                data.Remove(item);

                sceneBuildManager.DeletePathData(item.PathGUID);

                for (int i = 0; i < data.Count; i++)
                {
                    data[i].ObjectIndex = i;
                }

                this.DataSource = data;
            }

            
        }
    }

    public void CreateObjectPath(int index)
    {
        if (sceneBuildManager.isCreatingPath)
        {
            string msg = "Đang tạo đường di chuyển";
            sceneBuildManager.ShowInfoDialog(msg);

            return;
        }
        if (sceneBuildManager.popManager.gameObject.activeSelf)
        {
            string msg = "Tắt cửa sổ thông số điểm";
            sceneBuildManager.ShowInfoDialog(msg);

            return;
        }

        StartCoroutine(CreateObjectPathAsync(index));
    }

    IEnumerator CreateObjectPathAsync(int index)
    {
        var item = data.Find(i => i.ObjectIndex == index);
        if (item != null)
        {
            bool goCreatePath = false;
            if (data[index].HavePath)
            {
                string msg = string.Format("Bạn có muốn TẠO LẠI đường di chuyển của đối tượng \"{0}\" không?", item.ObjectName);
                sceneBuildManager.ShowConfirmDialog(msg);

                while (sceneBuildManager.confirmStatus == ConfirmDialogStatus.Waiting)
                {
                    yield return null;
                }

                if (sceneBuildManager.confirmStatus == ConfirmDialogStatus.ConfirmYes)
                {
                    goCreatePath = true;
                }
            }
            else
                goCreatePath = true;

            if (goCreatePath)
            {
                var oldPathGUID = string.Empty;
                if (data[index].HavePath)
                    oldPathGUID = data[index].PathGUID;

                currentCreatePathIndex = index;
                currentCreatePathGUID = Guid.NewGuid().ToString().Replace("-", "_");

                sceneBuildManager.CreateNewPath(oldPathGUID, currentCreatePathGUID, CreatePathType.ObjectWithAction);
            }

            
        }
    }

    public void FinishCreateObjectPath(ObservableList<Vector3> points)
    {
        data[currentCreatePathIndex].HavePath = true;
        data[currentCreatePathIndex].PathGUID = currentCreatePathGUID;
        data[currentCreatePathIndex].IsShowPath = true;        
        data[currentCreatePathIndex].PointsData = new ObservableList<HWaypoint>();

        for (int i = 0; i < points.Count; i++)
        {
            data[currentCreatePathIndex].PointsData.Add(new HWaypoint()
            {
                posV3 = points[i],
                animAtPos = HumanAnim.Stand,
                timeWaitAtPos = 0,
                animWhileGoingToNextPos = HumanAnim.Walk,
                speedToNextPos = 1
            });
        }

        data[currentCreatePathIndex].CalculateApproxTimeOnPath();

        this.DataSource = data;
    }

    public void CancelCreateNewPath()
    {
        data[currentCreatePathIndex].HavePath = false;
        data[currentCreatePathIndex].PathGUID = string.Empty;
        data[currentCreatePathIndex].IsShowPath = false;
        data[currentCreatePathIndex].IsPreviewed = false;
        //data[currentCreatePathIndex].Points = new ObservableList<Vector3>();
        data[currentCreatePathIndex].PointsData = new ObservableList<HWaypoint>();

        this.DataSource = data;
    }

    public void ShowHideObjectPath(int objIndex)
    {
        data[objIndex].IsShowPath = !data[objIndex].IsShowPath;

        this.DataSource = data;

        sceneBuildManager.ShowHidePath(data[objIndex].PathGUID, data[objIndex].IsShowPath);
    }

    public void ViewFirstPointOnPath(int objIndex)
    {
        sceneBuildManager.ViewFirstPointOnPath(data[objIndex].PathGUID);
    }

    public void PreviewObjectPath(int objIndex, bool previewScenario = false)
    {
        if (previewScenario && data[objIndex].IsPreviewed)
            return;

        data[objIndex].IsPreviewed = !data[objIndex].IsPreviewed;
                
        this.DataSource = data;        

        sceneBuildManager.PreviewObjectOnPath(data[objIndex]);
    }

    public void EditPointData(int objIndex, int pointIndex)
    {
        var pointData = data[objIndex].PointsData[pointIndex];
        sceneBuildManager.EditPointOnPathData(objIndex, 
                                              data[objIndex].PathGUID,
                                              data[objIndex].PointsData.Count,
                                              pointIndex, 
                                              pointData, 
                                              string.IsNullOrWhiteSpace(data[objIndex].ObjectName) ? "##" : data[objIndex].ObjectName);
    }

    public void FinishEditPointOnPathData(int objIndex, int pointIndex, HWaypoint pointData)
    {
        data[objIndex].PointsData[pointIndex] = pointData;

        UpdateReachTimeOnPath(objIndex);
    }

    public void RefreshView(int objIndex, int pointIndex, bool isEditing)
    {
        foreach (var objComponent in Components)
        {
            objComponent.pointsListView.RefreshView(objIndex, pointIndex, isEditing);
        }
    }

    public void InsertPointOnPath(string pathGuid, int insertAtPointIndex, bool isBefore)
    {
        var obj = data.FirstOrDefault(x => x.PathGUID == pathGuid);
        if (obj != null)
        {
            obj.PointsData.Insert(isBefore ? insertAtPointIndex : insertAtPointIndex + 1,
                                  new HWaypoint
                                  {
                                      posV3 = obj.PointsData[insertAtPointIndex].posV3,
                                      animAtPos = HumanAnim.Stand,
                                      timeWaitAtPos = 0,
                                      animWhileGoingToNextPos = HumanAnim.Walk,
                                      speedToNextPos = 1
                                  });

            this.DataSource = data;

            //renew path
            sceneBuildManager.DeletePathData(pathGuid);

            var allPoints = obj.PointsData.Select(x => x.posV3).ToList();
            sceneBuildManager.wpCreator.CreatePathFromPoints(new ObservableList<Vector3>(allPoints), pathGuid, true);

            //update time on path
            UpdateReachTimeOnPath(obj.ObjectIndex);
        }
    }

    public void DeletePointOnPath(string pathGuid, int deletePointIndex)
    {
        var obj = data.FirstOrDefault(x => x.PathGUID == pathGuid);
        if (obj != null)
        {
            obj.PointsData.RemoveAt(deletePointIndex);

            this.DataSource = data;

            //renew path
            sceneBuildManager.DeletePathData(pathGuid);

            var allPoints = obj.PointsData.Select(x => x.posV3).ToList();
            sceneBuildManager.wpCreator.CreatePathFromPoints(new ObservableList<Vector3>(allPoints), pathGuid, true);

            sceneBuildManager.popManager.gameObject.SetActive(false);
        }
    }

    public void FollowByCamera(int objIndex)
    {
        foreach (var item in Components)
        {
            if (item.data.ObjectIndex != objIndex)
            {
                item.data.IsFollowByCamera = false;

                item.BtnFollowByCamera.gameObject.SetActive(true);
                item.BtnNotFollowByCamera.gameObject.SetActive(false);
            }
            else
            {
                item.data.IsFollowByCamera = !item.data.IsFollowByCamera;

                item.BtnFollowByCamera.gameObject.SetActive(!item.data.IsFollowByCamera);
                item.BtnNotFollowByCamera.gameObject.SetActive(item.data.IsFollowByCamera);

                sceneBuildManager.UpdateFollowedObjectInfo(item.data);
            }
        }
    }

    public string GetFollowedByCamObjGuid()
    {
        foreach (var item in DataSource)
        {
            if (item.IsFollowByCamera)
                return item.ObjectGUID;
        }

        return string.Empty;
    }

    public void UpdateReachTimeOnPath(int objIndex)
    {
        data[objIndex].CalculateApproxTimeOnPath();
    }

    public Vector3 GetPointPos(int objIndex, int pointIndex)
    {
        foreach (var obj in DataSource)
        {
            if (obj.ObjectIndex == objIndex && obj.PointsData.Count > pointIndex)
            {
                return obj.PointsData[pointIndex].posV3;
            }
        }

        return Vector3.zero;
    }
}
