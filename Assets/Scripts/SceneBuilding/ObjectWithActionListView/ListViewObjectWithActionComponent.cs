using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

public class ListViewObjectWithActionComponent : ListViewItem, IViewData<ListViewObjectWithActionItemDescription>
{
    [SerializeField]
    public InputField TxtName;

    [SerializeField]
    public RawImage PrefabIcon;

    [SerializeField]
    public Button BtnCreatPath;

    [SerializeField]
    public Button BtnShowHidePath;

    [SerializeField]
    public Button BtnViewFirstPoint;

    [SerializeField]
    public Button BtnPreviewPath;

    [SerializeField]
    public Image HavePathIcon;

    [SerializeField]
    public Image NotHavePathIcon;

    [SerializeField]
    public Switch SwitchIncludeInPreview;

    [SerializeField]
    public Button BtnFollowByCamera;
    [SerializeField]
    public Button BtnNotFollowByCamera;

    [Space(10)]
    public ListViewObjectWithAction mainListView;
    public ListViewObjectWithActionItemDescription data;
    public ListViewPointOnPath pointsListView;

    public void SetData(ListViewObjectWithActionItemDescription item)
    {   
        if (item == null)
        {
            HavePathIcon.gameObject.SetActive(false);
            NotHavePathIcon.gameObject.SetActive(true);
            PrefabIcon.gameObject.SetActive(false);
        }
        else
        {
            data = item;            

            TxtName.text = data.ObjectName;

            if (!string.IsNullOrEmpty(data.ObjectPrefabId))
            {
                var icon = Resources.Load<Texture2D>(Helper.PrefabImages_ResourcesFolder + data.ObjectPrefabId);
                if (icon != null)
                {
                    PrefabIcon.gameObject.SetActive(true);
                    PrefabIcon.texture = icon;
                }
            }
            else
            {
                PrefabIcon.gameObject.SetActive(false);
                PrefabIcon.texture = null;
            }

            //BtnCreatPath.interactable = !data.HavePath;
            BtnShowHidePath.interactable = data.HavePath;
            BtnViewFirstPoint.interactable = data.HavePath;
            BtnPreviewPath.interactable = data.HavePath;

            HavePathIcon.gameObject.SetActive(data.HavePath);
            NotHavePathIcon.gameObject.SetActive(!data.HavePath);

            BtnCreatPath.GetComponentInChildren<Text>().text = data.HavePath ? "Tạo lại" : "Tạo đường";
            BtnShowHidePath.GetComponentInChildren<Text>().text = data.IsShowPath ? "Ẩn" : "Hiện";
            BtnPreviewPath.GetComponentInChildren<Text>().text = data.IsPreviewed ? "Dừng" : "Xem thử";

            SwitchIncludeInPreview.IsOn = data.IsIncludeInPreview;

            BtnFollowByCamera.gameObject.SetActive(!data.IsFollowByCamera);
            BtnNotFollowByCamera.gameObject.SetActive(data.IsFollowByCamera);

            if (data.PointsData == null) data.PointsData = new ObservableList<HWaypoint>();
            var lst = new ObservableList<ListViewPointOnPathItemDescription>();
            for (int i = 0; i < data.PointsData.Count; i++)
            {
                lst.Add(new ListViewPointOnPathItemDescription
                {
                    PointIndex = i
                    
                });
            }
            pointsListView.DataSource = lst;
            pointsListView.belongToObjectIndex = data.ObjectIndex;
        }
    }

    public void DeleteRecord()
    {
        mainListView.DeleteObjectWithActionRecord(data.ObjectIndex);
    }

    public void TxtNameChange(string value)
    {
        data.ObjectName = value;
    }

    public void SwitchIIPChanged()
    {
        data.IsIncludeInPreview = SwitchIncludeInPreview.IsOn;
    }

    public void FollowByCamera()
    {
        mainListView.FollowByCamera(data.ObjectIndex);
    }

    public void CreateObjectPath()
    {   
        mainListView.CreateObjectPath(data.ObjectIndex);
    }

    public void ShowHideObjectPath()
    {
        mainListView.ShowHideObjectPath(data.ObjectIndex);
    }

    public void ViewFirstPointOnPath()
    {
        mainListView.ViewFirstPointOnPath(data.ObjectIndex);
    }

    public void PreviewObjectPath()
    {
        mainListView.PreviewObjectPath(data.ObjectIndex);
    }
}
