using System;
using UIWidgets;
using UnityEngine;


public class ListViewPointOnPath : ListViewCustom<ListViewPointOnPathComponent, ListViewPointOnPathItemDescription>
{
    readonly Comparison<ListViewPointOnPathItemDescription> itemsComparison = (x, y) => UtilitiesCompare.Compare(x.PointIndex, y.PointIndex);

    bool isListViewPointOnPathInited;

    public ListViewObjectWithAction listViewObject;
    public int belongToObjectIndex;

    /// <summary>
    /// Set items comparison.
    /// </summary>
    public override void Init()
    {
        if (isListViewPointOnPathInited)
        {
            return;
        }
        
        isListViewPointOnPathInited = true;
        
        base.Init();
        DataSource.Comparison = itemsComparison;
    }

    public void EditPointData(int pointIndex)
    {
        listViewObject.EditPointData(belongToObjectIndex, pointIndex);
    }

    public void RefreshView(int objIndex, int pointIndex, bool isEditing)
    {
        foreach (var pointComponent in Components)
        {
            pointComponent.data.IsInEditMode = belongToObjectIndex == objIndex && pointComponent.data.PointIndex == pointIndex && isEditing;

            //if editing some point, disable all others
            if (isEditing && !pointComponent.data.IsInEditMode)
                pointComponent.data.ShouldDisable = true;
            else if (!isEditing)
                pointComponent.data.ShouldDisable = false;
        }

        UpdateView();
    }
}
