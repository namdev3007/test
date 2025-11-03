using UIWidgets;
using UnityEngine;
using UnityEngine.UI;


public class ListViewPointOnPathComponent : ListViewItem, IViewData<ListViewPointOnPathItemDescription>
{
    [SerializeField]
    public Button PointButton;

    [Space(10)]
    public ListViewPointOnPath mainListView;
    public ListViewPointOnPathItemDescription data;

    public void SetData(ListViewPointOnPathItemDescription item)
    {
        if (item == null)
        {
        }
        else
        {
            data = item;

            if (!data.IsInEditMode)
                PointButton.GetComponentInChildren<Text>().text = string.Format("Điểm {0}", data.PointIndex + 1);
            else
                PointButton.GetComponentInChildren<Text>().text = string.Format("<color=#ff0000>Điểm {0}</color>", data.PointIndex + 1);

            PointButton.interactable = !data.ShouldDisable;
        }
    }

    public void EditPointData()
    {
        mainListView.EditPointData(data.PointIndex);
    }
}
