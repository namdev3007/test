using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

public class ListViewWalkingPeopleComponent : ListViewItem, IViewData<ListViewWalkingPeopleItemDescription>
{
    [SerializeField]
    public InputField TxtNote;

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
    public Slider SliderDensity;

    [SerializeField]
    public Toggle CreateAlongPathToggle;

    [SerializeField]
    public Toggle IsLoopToggle;

    [SerializeField]
    public Toggle IsReverseToggle;

    [SerializeField]
    public InputField TxtTimeInterval;



    [Space(10)]
    public ListViewWalkingPeople mainListView;
    public ListViewWalkingPeopleItemDescription data;

    public void SetData(ListViewWalkingPeopleItemDescription item)
    {
        if (item == null)
        {
            HavePathIcon.gameObject.SetActive(false);
            NotHavePathIcon.gameObject.SetActive(true);
        }
        else
        {
            data = item;

            TxtNote.text = data.Note;
            SliderDensity.value = data.Density;
            TxtTimeInterval.text = data.TimeInterval.ToString();
            CreateAlongPathToggle.isOn = data.CreateAlongPath;
            IsLoopToggle.isOn = data.IsLoop;
            IsReverseToggle.isOn = data.IsReverse;

            //BtnCreatPath.interactable = !data.HavePath;
            BtnShowHidePath.interactable = data.HavePath;
            BtnViewFirstPoint.interactable = data.HavePath;
            BtnPreviewPath.interactable = data.HavePath;

            HavePathIcon.gameObject.SetActive(data.HavePath);
            NotHavePathIcon.gameObject.SetActive(!data.HavePath);

            BtnCreatPath.GetComponentInChildren<Text>().text = data.HavePath ? "Tạo lại" : "Tạo đường";
            BtnShowHidePath.GetComponentInChildren<Text>().text = data.IsShowed ? "Ẩn" : "Hiện";
            BtnPreviewPath.GetComponentInChildren<Text>().text = data.IsPreviewed ? "Dừng" : "Xem thử";
        }
    }

    public void DeleteRecord()
    {
        mainListView.DeleteWalkingPeopleRecord(data.WalkingPeopleIndex);
    }

    public void TxtNoteChange(string value)
    {
        data.Note = value;
    }

    public void DensitySliderChange(float value)
    {
        data.Density = value;
    }

    public void TxtTimeIntervalChange(string value)
    {
        if (!float.TryParse(value, out data.TimeInterval))
            TxtTimeInterval.text = string.IsNullOrEmpty(value) ? "0" : value.Substring(0, value.Length - 1);
    }

    public void CreateAlongPathToggleChange(bool value)
    {
        data.CreateAlongPath = value;
    }

    public void LoopToggleChange(bool value)
    {
        data.IsLoop = value;
    }

    public void ReverseToggleChange(bool value)
    {
        data.IsReverse = value;
    }

    public void CreateNewPath()
    {
        mainListView.CreateNewPath(data.WalkingPeopleIndex);
    }

    public void ShowHidePath()
    {
        mainListView.ShowHidePath(data.WalkingPeopleIndex);
    }

    public void ViewFirstPointOnPath()
    {
        mainListView.ViewFirstPointOnPath(data.WalkingPeopleIndex);
    }

    public void PreviewPath()
    {   
        mainListView.PreviewPath(data.WalkingPeopleIndex);
    }
}
