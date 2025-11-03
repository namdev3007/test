using System;
using System.Collections;
using UIWidgets;
using UnityEngine;

public class ListViewWalkingPeople : ListViewCustom<ListViewWalkingPeopleComponent, ListViewWalkingPeopleItemDescription>
{
    readonly Comparison<ListViewWalkingPeopleItemDescription> itemsComparison = (x, y) => UtilitiesCompare.Compare(x.WalkingPeopleIndex, y.WalkingPeopleIndex);

    bool isListViewWalkingPeopleInited;

    public ObservableList<ListViewWalkingPeopleItemDescription> data = new ObservableList<ListViewWalkingPeopleItemDescription>();

    public SceneBuildingManager sceneBuildManager;
    public string currentCreatePathGUID;
    public int currentCreatePathIndex;

    /// <summary>
    /// Set items comparison.
    /// </summary>
    public override void Init()
    {
        if (isListViewWalkingPeopleInited)
        {
            return;
        }

        isListViewWalkingPeopleInited = true;

        base.Init();
        DataSource.Comparison = itemsComparison;
    }

    public void LoadDataFromList(ObservableList<ListViewWalkingPeopleItemDescription> list)
    {
        data = list;

        this.DataSource = data;
    }

    public void AddNewWalkingPeople()
    {
        for (int i = 0; i < data.Count; i++)
        {
            data[i].WalkingPeopleIndex = i;
        }

        data.Add(new ListViewWalkingPeopleItemDescription() { WalkingPeopleIndex = data.Count });

        this.DataSource = data;
    }

    public void DeleteWalkingPeopleRecord(int index)
    {
        StartCoroutine(DeleteWalkingPeopleRecordAsync(index));

        //var item = data.Find(i => i.WalkingPeopleIndex == index);
        //if (item != null)
        //{
        //    data.Remove(item);

        //    sceneBuildManager.DeletePathData(item.PathGUID);

        //    for (int i = 0; i < data.Count; i++)
        //    {
        //        data[i].WalkingPeopleIndex = i;
        //    }

        //    this.DataSource = data;
        //}
    }

    IEnumerator DeleteWalkingPeopleRecordAsync(int index)
    {
        var item = data.Find(i => i.WalkingPeopleIndex == index);
        if (item != null)
        {
            string msg = string.Format("Bạn có muốn xóa tuyến đường \"{0}\" không?", item.Note);
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
                    data[i].WalkingPeopleIndex = i;
                }

                this.DataSource = data;
            }
        }
    }

    public void CreateNewPath(int index)
    {
        if (sceneBuildManager.isCreatingPath) return;

        var oldPathGUID = string.Empty;
        if (data[index].HavePath)
            oldPathGUID = data[index].PathGUID;

        currentCreatePathIndex = index;
        currentCreatePathGUID = Guid.NewGuid().ToString().Replace("-","_");

        sceneBuildManager.CreateNewPath(oldPathGUID, currentCreatePathGUID, CreatePathType.WalkingPeople);
    }

    public void FinishCreateNewPath(ObservableList<Vector3> points)
    {
        data[currentCreatePathIndex].HavePath = true;
        data[currentCreatePathIndex].PathGUID = currentCreatePathGUID;
        data[currentCreatePathIndex].IsShowed = true;
        data[currentCreatePathIndex].Points = points;

        this.DataSource = data;
    }

    public void CancelCreateNewPath()
    {
        data[currentCreatePathIndex].HavePath = false;
        data[currentCreatePathIndex].PathGUID = string.Empty;
        data[currentCreatePathIndex].IsShowed = false;
        data[currentCreatePathIndex].IsPreviewed = false;
        data[currentCreatePathIndex].Points = new ObservableList<Vector3>();

        this.DataSource = data;
    }

    public void ShowHidePath(int index)
    {
        data[index].IsShowed = !data[index].IsShowed;

        this.DataSource = data;

        sceneBuildManager.ShowHidePath(data[index].PathGUID, data[index].IsShowed);
    }

    public void ViewFirstPointOnPath(int index)
    {
        sceneBuildManager.ViewFirstPointOnPath(data[index].PathGUID);
    }

    public void PreviewPath(int index, bool previewScenario = false)
    {
        if (previewScenario && data[index].IsPreviewed)
            return;

        data[index].IsPreviewed = !data[index].IsPreviewed;

        this.DataSource = data;

        sceneBuildManager.PreviewPath(
            data[index].PathGUID,
            data[index].IsPreviewed,
            data[index].CreateAlongPath,
            data[index].IsLoop,
            data[index].IsReverse,
            data[index].Density,
            data[index].TimeInterval);
            
    }
}
